namespace SharpNeedle.Resource;

using SharpNeedle.Framework.HedgehogEngine.Mirage;

public static class ResourceExtensions
{
    public static string GetIdentifier(this IResource res)
    {
        return ResourceManager.GetIdentifier(res.GetType());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Open<T>(this IResourceManager manager, string path, bool resolveDepends = true) where T : IResource, new()
    {
        IFile file = FileSystem.Open(path);
        return manager.Open<T>(file, resolveDepends);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IResource Open(this IResourceManager manager, string path, bool resolveDepends = true)
    {
        IFile file = FileSystem.Open(path);
        return manager.Open(file, resolveDepends);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Read(this IResource resource, string path, bool resolveDepends = true)
    {
        IFile file = FileSystem.Open(path);
        resource.Read(file);

        if(resolveDepends)
        {
            resource.ResolveDependencies(new DirectoryResourceResolver(file.Parent));
        }
    }

    public static void Write(this IResource resource, string path, bool saveDepends = true)
    {
        using IFile file = FileSystem.Create(path);
        resource.Write(file);

        if(saveDepends)
        {
            resource.WriteDependencies(file.Parent);
        }
    }

    public static void Write(this SampleChunkResource resource, string path, bool writeNodes, bool saveDepends = true)
    {
        using IFile file = FileSystem.Create(path);
        resource.Write(file, writeNodes);

        if(saveDepends)
        {
            resource.WriteDependencies(file.Parent);
        }
    }
}
