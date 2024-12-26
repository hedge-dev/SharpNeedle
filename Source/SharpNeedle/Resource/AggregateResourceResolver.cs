namespace SharpNeedle.Resource;

using SharpNeedle.IO;

public class AggregateResourceResolver : List<IResourceResolver>, IResourceResolver
{
    public TRes? Open<TRes>(string fileName) where TRes : IResource, new()
    {
        foreach(IResourceResolver resolver in this)
        {
            TRes? res = resolver.Open<TRes>(fileName);
            if(res != null)
            {
                return res;
            }
        }

        return default;
    }

    public IFile? GetFile(string filename)
    {
        foreach(IResourceResolver resolver in this)
        {
            IFile? file = resolver.GetFile(filename);
            if(file != null)
            {
                return file;
            }
        }

        return null;
    }
}