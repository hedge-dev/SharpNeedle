namespace SharpNeedle.Resource;

using System.Reflection;

public class ResourceManager : IResourceManager
{
    private static readonly Dictionary<Type, string> _types = [];
    private static readonly Dictionary<string, NeedleResourceAttribute> _resourceTypes = [];

    public static ResourceManager Instance => Singleton.GetInstance<ResourceManager>()!;

    // Use weak references so resources can get garbage collected and we won't have to worry about leaving them open
    private readonly Dictionary<string, WeakReference<IResource>> _resources = [];
    private readonly ConditionalWeakTable<IResource, string> _resourceTable = [];


    static ResourceManager()
    {
        RegisterResourceTypes(typeof(ResourceManager).Assembly);

        ResourceManager instance = new();
        Singleton.SetInstance(instance);
        Singleton.SetInstance<IResourceManager>(instance);
    }

    public static void RegisterResourceTypes(Assembly assembly, bool ignoreRegistered = true)
    {
        foreach(Type type in assembly.GetTypes().Where(t => t.GetCustomAttribute<NeedleResourceAttribute>() != null))
        {
            NeedleResourceAttribute attribute = type.GetCustomAttribute<NeedleResourceAttribute>()!;
            attribute.Owner = type;

            if(_resourceTypes.ContainsKey(attribute.Id) && ignoreRegistered)
            {
                continue;
            }

            foreach(MethodInfo method in type.GetMethods())
            {
                ResourceCheckFunctionAttribute? checkAttribute = method.GetCustomAttribute<ResourceCheckFunctionAttribute>();
                if(checkAttribute == null)
                {
                    continue;
                }

                attribute.CheckResourceFunc = (Func<IFile, bool>)method.CreateDelegate(typeof(Func<IFile, bool>));
                break;
            }

            _types.Add(type, attribute.Id);
            _resourceTypes.Add(attribute.Id, attribute);
        }
    }

    public static NeedleResourceAttribute? DetectType(IFile file)
    {
        foreach(NeedleResourceAttribute value in _resourceTypes.Values)
        {
            if(value.CheckResource(file))
            {
                return value;
            }
        }

        return null;
    }

    public static string? GetIdentifier(Type type)
    {
        _types.TryGetValue(type, out string? value);
        return value;
    }

    public static ResourceType? GetType(Type type)
    {
        if(_types.TryGetValue(type, out string? value))
        {
            return _resourceTypes[value].Type;
        }

        return null;
    }


    public void Dispose()
    {
        foreach(WeakReference<IResource> resource in _resources.Values)
        {
            if(resource.TryGetTarget(out IResource? res))
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
        if(_resources.TryGetValue(path, out WeakReference<IResource>? resRef))
        {
            if(resRef.TryGetTarget(out IResource? cacheRes))
            {
                return (T)cacheRes;
            }

            _resources.Remove(path);
        }


        res.Read(file);
        _resources.Add(path, new WeakReference<IResource>(res));
        _resourceTable.Add(res, path);
        if(resolveDepends)
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
        Type type = DetectType(file)?.Owner ?? typeof(ResourceRaw);
        IResource res = (IResource)Activator.CreateInstance(type)!;
        return OpenBase(file, ref res, resolveDepends);
    }

    public bool IsOpen(string path)
    {
        return _resources.TryGetValue(path, out WeakReference<IResource>? refRes) && refRes.TryGetTarget(out _);
    }

    public void Close(IResource res)
    {
        if(!_resourceTable.TryGetValue(res, out string? key))
        {
            return;
        }

        _resources.Remove(key);
        _resourceTable.Remove(res);
        res.Dispose();
    }

}