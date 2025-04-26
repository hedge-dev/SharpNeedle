namespace SharpNeedle.Resource;

using System.Collections.Concurrent;
using System.Collections.Generic;

public class ResourceManager : IResourceManager
{
    public static ResourceManager Instance => Singleton.GetInstance<ResourceManager>()!;

    // Use weak references so resources can get garbage collected and we won't have to worry about leaving them open
    private readonly ConcurrentDictionary<string, WeakReference<IResource>> _resources = [];
    private readonly HashSet<IResource> _readingResources = [];

    static ResourceManager()
    {
        ResourceManager instance = new();
        Singleton.SetInstance(instance);
        Singleton.SetInstance<IResourceManager>(instance);
    }


    public void Dispose()
    {
        foreach (WeakReference<IResource> resource in _resources.Values)
        {
            if (resource.TryGetTarget(out IResource? res))
            {
                res.Dispose();
            }
        }

        _resources.Clear();
    }


    private IResource OpenBase(IFile file, Type resourceType, bool resolveDepends = true)
    {
        Func<IResource, bool> addUpdateFunc;

        if (!_resources.TryGetValue(file.Path, out WeakReference<IResource>? resRef))
        {
            addUpdateFunc = (res) => _resources.TryAdd(file.Path, new(res));
        }
        else if(!resRef.TryGetTarget(out IResource? cacheRes))
        {
            addUpdateFunc = (res) => _resources.TryUpdate(file.Path, new(res), resRef);
        }
        else
        {
            if(_readingResources.Contains(cacheRes))
            {
                System.Threading.SpinWait.SpinUntil(() => !_readingResources.Contains(cacheRes));
            }

            return cacheRes;
        }

        IResource result = (IResource)Activator.CreateInstance(resourceType)!;
        
        lock (_readingResources) 
        { 
            _readingResources.Add(result); 
        }

        if(!addUpdateFunc(result))
        {
            lock (_readingResources) 
            { 
                _readingResources.Remove(result); 
            }

            // If the add fails, this means another thread was quicker than us.

            // Return the data added by the winning thread.
            if (!_resources.TryGetValue(file.Path, out resRef))
            {
                throw new Exception("TryGetValue return value is somehow false!");
            }

            if (!resRef.TryGetTarget(out IResource? cacheRes))
            {
                throw new Exception("Added resource somehow doesn't exist!");
            }

            System.Threading.SpinWait.SpinUntil(() => !_readingResources.Contains(cacheRes));
            return cacheRes;    
        }

        result.Read(file);

        if (resolveDepends)
        {
            result.ResolveDependencies(new DirectoryResourceResolver(file.Parent, this));
        }

        lock (_readingResources)
        {
            _readingResources.Remove(result);
        }

        return result;
    }

    public T Open<T>(IFile file, bool resolveDepends = true) where T : IResource, new()
    {
        return (T)OpenBase(file, typeof(T), resolveDepends);
    }

    public IResource Open(IFile file, bool resolveDepends = true)
    {
        Type type = ResourceTypeManager.DetectType(file)?.Owner ?? typeof(ResourceRaw);
        return OpenBase(file, type, resolveDepends);
    }

    public bool IsOpen(string path)
    {
        return _resources.TryGetValue(path, out WeakReference<IResource>? refRes) && refRes.TryGetTarget(out _);
    }
}