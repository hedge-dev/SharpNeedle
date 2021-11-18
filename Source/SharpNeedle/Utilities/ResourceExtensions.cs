namespace SharpNeedle.Utilities;

public static class ResourceExtensions
{
    public static T Open<T>(this IResourceManager manager, string path, bool resolveDepends = true) where T : IResource, new()
    {
        var file = FileSystem.Open(path);
        return manager.Open<T>(file, resolveDepends);
    }

    public static IResource Open(this IResourceManager manager, string path, bool resolveDepends = true)
    {
        var file = FileSystem.Open(path);
        return manager.Open(file, resolveDepends);
    }

    public static void Read(this IResource resource, string path, bool resolveDepends = true)
    {
        var file = FileSystem.Open(path);
        resource.Read(file);

        if (resolveDepends)
            resource.ResolveDependencies(file.Parent);
    }
}

public static class ResourceUtility
{
    public static TRes Open<TRes>(string path, bool resolveDepends = true) where TRes : IResource, new()
    {
        var file = FileSystem.Open(path);
        if (file == null)
            return default;

        var res = new TRes();
        res.Read(file);
        if (resolveDepends)
            res.ResolveDependencies(file.Parent);

        return res;
    }

    public static IResource Open(string path, bool resolveDepends = true)
    {
        var file = FileSystem.Open(path);
        if (file == null)
            return null;

        var resType = ResourceManager.DetectType(file).Owner;
        var res = (IResource)Activator.CreateInstance(resType);
        res.Read(file);
        
        if (resolveDepends)
            res.ResolveDependencies(file.Parent);

        return res;
    }
}