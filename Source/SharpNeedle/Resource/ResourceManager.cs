namespace SharpNeedle.Resource;

using System.Collections.Concurrent;

public class ResourceManager : IResourceManager
{
    public static ResourceManager Instance => Singleton.GetInstance<ResourceManager>()!;

    // Use weak references so resources can get garbage collected and we won't have to worry about leaving them open
    private readonly ConcurrentDictionary<string, WeakReference<IResource>> _resources = [];
    private readonly ConditionalWeakTable<IResource, string> _resourceTable = [];


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
        _resourceTable.Clear();
    }


    private IResource OpenBase(IFile file, Type resourceType, bool resolveDepends = true)
    {
        if (_resources.TryGetValue(file.Path, out WeakReference<IResource>? resRef)
            && resRef.TryGetTarget(out IResource? cacheRes))
        {
            return cacheRes;
        }

        WeakReference<IResource> AddFunc(string path)
        {
            IResource res = (IResource)Activator.CreateInstance(resourceType)!;
            res.Read(file);

            _resourceTable.Add(res, file.Path);

            return new(res);
        }

        resRef = _resources.AddOrUpdate(file.Path, AddFunc, (k, p) => AddFunc(k));

        if (!resRef.TryGetTarget(out cacheRes))
        {
            throw new NullReferenceException("Result somehow null");
        }

        if (resolveDepends)
        {
            cacheRes.ResolveDependencies(new DirectoryResourceResolver(file.Parent, this));
        }

        return cacheRes;
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

    public void Close(IResource res)
    {
        if (!_resourceTable.TryGetValue(res, out string? key))
        {
            return;
        }

        if(_resources.TryRemove(key, out _))
        {
            _resourceTable.Remove(res);
        }

        res.Dispose();
    }

}