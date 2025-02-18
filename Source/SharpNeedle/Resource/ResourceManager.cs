namespace SharpNeedle.Resource;

public class ResourceManager : IResourceManager
{
    public static ResourceManager Instance => Singleton.GetInstance<ResourceManager>()!;

    // Use weak references so resources can get garbage collected and we won't have to worry about leaving them open
    private readonly Dictionary<string, WeakReference<IResource>> _resources = [];
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


    private T OpenBase<T>(IFile file, ref T res, bool resolveDepends = true) where T : IResource
    {
        string path = file.Path;
        if (_resources.TryGetValue(path, out WeakReference<IResource>? resRef))
        {
            if (resRef.TryGetTarget(out IResource? cacheRes))
            {
                return (T)cacheRes;
            }

            _resources.Remove(path);
        }


        res.Read(file);
        _resources.Add(path, new WeakReference<IResource>(res));
        _resourceTable.Add(res, path);
        if (resolveDepends)
        {
            res.ResolveDependencies(new DirectoryResourceResolver(file.Parent, this));
        }

        return res;
    }

    public T Open<T>(IFile file, bool resolveDepends = true) where T : IResource, new()
    {
        T res = new();
        return OpenBase(file, ref res, resolveDepends);
    }

    public IResource Open(IFile file, bool resolveDepends = true)
    {
        Type type = ResourceTypeManager.DetectType(file)?.Owner ?? typeof(ResourceRaw);
        IResource res = (IResource)Activator.CreateInstance(type)!;
        return OpenBase(file, ref res, resolveDepends);
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

        _resources.Remove(key);
        _resourceTable.Remove(res);
        res.Dispose();
    }

}