namespace SharpNeedle;
using System.IO;

public struct DirectoryResourceResolver : IResourceResolver
{
    public IResourceManager Manager;
    public IDirectory Directory;

    public DirectoryResourceResolver(IDirectory dir, IResourceManager manager = null)
    {
        Manager = manager;
        Directory = dir;
    }

    public readonly TRes Open<TRes>(string fileName) where TRes : IResource, new()
    {
        IFile file = Directory[fileName];
        if(file == null)
        {
            throw new FileNotFoundException($"File \"{fileName}\" not found", Path.Join(Directory.Path, fileName));
        }

        TRes res;
        if(Manager != null)
        {
            res = Manager.Open<TRes>(file, false);
            res.ResolveDependencies(this);
            return res;
        }

        res = new TRes();
        res.Read(file);
        res.ResolveDependencies(this);
        return res;
    }
}