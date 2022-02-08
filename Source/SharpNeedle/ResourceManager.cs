namespace SharpNeedle;
using System.Reflection;

public class ResourceManager : IResourceManager
{
    private static readonly Dictionary<Type, string> mTypes = new();
    private static readonly Dictionary<string, NeedleResourceAttribute> mResourceTypes = new();

    // Use weak references so resources can get garbage collected and we won't have to worry about leaving them open
    private readonly Dictionary<string, WeakReference<IResource>> mResources = new();
    private readonly ConditionalWeakTable<IResource, string> mResourceTable = new();

    public static ResourceManager Instance => Singleton.GetInstance<ResourceManager>();

    static ResourceManager()
    {
        RegisterResourceTypes(typeof(ResourceManager).Assembly);
        
        var instance = new ResourceManager();
        Singleton.SetInstance(instance);
        Singleton.SetInstance<IResourceManager>(instance);
    }

    private T OpenBase<T>(IFile file, ref T res, bool resolveDepends = true) where T : IResource
    {
        var path = file.Path;
        if (mResources.TryGetValue(path, out var resRef))
        {
            if (resRef.TryGetTarget(out var cacheRes))
                return (T)cacheRes;

            mResources.Remove(path);
        }


        res.Read(file);
        mResources.Add(path, new WeakReference<IResource>(res));
        mResourceTable.Add(res, path);
        if (resolveDepends)
            res.ResolveDependencies(new DirectoryResourceResolver(file.Parent, this));

        return res;
    }

    public T Open<T>(IFile file, bool resolveDepends = true) where T : IResource, new()
    {
        var res = new T();
        return OpenBase(file, ref res, resolveDepends);
    }

    public IResource Open(IFile file, bool resolveDepends = true)
    {
        var type = DetectType(file)?.Owner ?? typeof(ResourceRaw);
        var res = (IResource)Activator.CreateInstance(type);
        return OpenBase(file, ref res, resolveDepends);
    }

    public bool IsOpen(string path)
        => mResources.TryGetValue(path, out var refRes) && refRes.TryGetTarget(out _);

    public void Close(IResource res)
    {
        if (!mResourceTable.TryGetValue(res, out var key))
            return;

        mResources.Remove(key);
        mResourceTable.Remove(res);
        res.Dispose();
    }

    public static void RegisterResourceTypes(Assembly assembly, bool ignoreRegistered = true)
    {
        foreach (var type in assembly.GetTypes().Where(t => t.GetCustomAttribute<NeedleResourceAttribute>() != null))
        {
            var attribute = type.GetCustomAttribute<NeedleResourceAttribute>();
            attribute.Owner = type;

            if (mResourceTypes.ContainsKey(attribute.Id) && ignoreRegistered)
                continue;

            foreach (var method in type.GetMethods())
            {
                var checkAttribute = method.GetCustomAttribute<ResourceCheckFunctionAttribute>();
                if (checkAttribute == null)
                    continue;

                attribute.CheckResourceFunc = (Func<IFile, bool>)method.CreateDelegate(typeof(Func<IFile, bool>));
                break;
            }
            mTypes.Add(type, attribute.Id);
            mResourceTypes.Add(attribute.Id, attribute);
        }
    }

    public static NeedleResourceAttribute DetectType(IFile file)
    {
        foreach (var value in mResourceTypes.Values)
        {
            if (value.CheckResource(file))
                return value;
        }
        return null;
    }

    public static string GetIdentifier(Type type)
        => mTypes.TryGetValue(type, out string value) ? value : null;

    public static ResourceType? GetType(Type type)
    {
        if (mTypes.TryGetValue(type, out string value))
            return mResourceTypes[value].Type;

        return null;
    }

    public void Dispose()
    {
        foreach (var resource in mResources.Values)
        {
            if (resource.TryGetTarget(out var res))
                res.Dispose();
        }

        mResources.Clear();
        mResourceTable.Clear();
    }
}