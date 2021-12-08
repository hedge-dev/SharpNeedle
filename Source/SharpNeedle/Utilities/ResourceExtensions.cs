namespace SharpNeedle.Utilities;
using HedgehogEngine.Mirage;

public static class ResourceExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Open<T>(this IResourceManager manager, string path, bool resolveDepends = true) where T : IResource, new()
    {
        var file = FileSystem.Open(path);
        return manager.Open<T>(file, resolveDepends);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResource Open(this IResourceManager manager, string path, bool resolveDepends = true)
    {
        var file = FileSystem.Open(path);
        return manager.Open(file, resolveDepends);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Read(this IResource resource, string path, bool resolveDepends = true)
    {
        var file = FileSystem.Open(path);
        resource.Read(file);

        if (resolveDepends)
            resource.ResolveDependencies(new DirectoryResourceResolver(file.Parent));
    }

    public static void Write(this IResource resource, string path, bool saveDepends = true)
    {
        using var file = FileSystem.Create(path);
        resource.Write(file);

        if (saveDepends)
            resource.WriteDependencies(file.Parent);
    }

    public static void Write(this SampleChunkResource resource, string path, bool writeNodes, bool saveDepends = true)
    {
        using var file = FileSystem.Create(path);
        resource.Write(file, writeNodes);

        if (saveDepends)
            resource.WriteDependencies(file.Parent);
    }
}

public static class ResourceUtility
{
    public static TRes Open<TRes>(string path, bool resolveDepends = true) where TRes : IResource, new()
    {
        var file = FileSystem.Open(path);
        if (file == null)
            return default;

        return ResourceManager.Instance.Open<TRes>(file, resolveDepends);
    }

    public static IResource Open(string path, bool resolveDepends = true)
    {
        var file = FileSystem.Open(path);
        if (file == null)
            return null;

        return ResourceManager.Instance.Open(file, resolveDepends);
    }
}