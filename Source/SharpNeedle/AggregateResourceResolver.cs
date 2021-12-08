namespace SharpNeedle;

public class AggregateResourceResolver : List<IResourceResolver>, IResourceResolver
{
    public TRes Open<TRes>(string fileName) where TRes : IResource, new()
    {
        foreach (var resolver in this)
        {
            var res = resolver.Open<TRes>(fileName);
            if (res != null)
                return res;
        }

        return default;
    }
}