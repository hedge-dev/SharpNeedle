namespace SharpNeedle.Resource;
public interface IResourceResolver
{
    TRes? Open<TRes>(string fileName) where TRes : IResource, new();

    IFile? GetFile(string filename);
}