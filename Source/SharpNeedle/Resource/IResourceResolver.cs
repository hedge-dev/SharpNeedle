namespace SharpNeedle.Resource;

public interface IResourceResolver
{
    TRes? Open<TRes>(string fileName, bool resolveDependencies = true) where TRes : IResource, new();

    IFile? GetFile(string filename);
}