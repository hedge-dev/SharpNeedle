namespace SharpNeedle.Resource;

public interface IResourceManager : IDisposable
{
    IResource Open(IFile file, bool resolveDepends = true);

    T Open<T>(IFile file, bool resolveDepends = true) where T : IResource, new();


    bool IsOpen(string path);
}