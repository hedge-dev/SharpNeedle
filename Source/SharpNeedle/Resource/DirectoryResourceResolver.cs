namespace SharpNeedle.Resource;

public struct DirectoryResourceResolver : IResourceResolver
{
    public IDirectory Directory { get; set; }

    public IResourceManager? Manager { get; set; }


    public DirectoryResourceResolver(IDirectory dir, IResourceManager? manager = null)
    {
        Manager = manager;
        Directory = dir;
    }


    public readonly TRes? Open<TRes>(string fileName, bool resolveDependencies = true) where TRes : IResource, new()
    {
        IFile? file = GetFile(fileName);
        if(file == null)
        {
            return default;
        }

        TRes res;
        if(Manager != null)
        {
            res = Manager.Open<TRes>(file, false);
        }
        else
        {
            res = new();
            res.Read(file);
        }

        if(resolveDependencies)
        {
            res.ResolveDependencies(this);
        }

        return res;
    }

    public readonly IFile? GetFile(string filename)
    {
        return Directory[filename];
    }
}