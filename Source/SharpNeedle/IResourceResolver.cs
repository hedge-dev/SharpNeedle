namespace SharpNeedle;

public interface IResourceResolver
{
    TRes Open<TRes>(string fileName) where TRes : IResource, new();
}