namespace SharpNeedle.Resource;

public static class ResourceUtility
{
    public static TRes? Open<TRes>(string path, bool resolveDepends = true) where TRes : IResource, new()
    {
        IFile? file = FileSystem.Instance.Open(path);
        if(file == null)
        {
            return default;
        }

        return ResourceManager.Instance.Open<TRes>(file, resolveDepends);
    }

    public static IResource? Open(string path, bool resolveDepends = true)
    {
        IFile? file = FileSystem.Instance.Open(path);
        if(file == null)
        {
            return null;
        }

        return ResourceManager.Instance.Open(file, resolveDepends);
    }
}
