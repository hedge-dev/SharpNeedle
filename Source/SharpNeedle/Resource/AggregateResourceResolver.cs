namespace SharpNeedle.Resource;

public class AggregateResourceResolver : List<IResourceResolver>, IResourceResolver
{
    public TRes Open<TRes>(string fileName) where TRes : IResource, new()
    {
        foreach(IResourceResolver resolver in this)
        {
            TRes res = resolver.Open<TRes>(fileName);
            if(res != null)
            {
                return res;
            }
        }

        return default;
    }
}