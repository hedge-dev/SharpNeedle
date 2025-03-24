namespace SharpNeedle.Resource;
using System;

public class ResourceResolveException : Exception
{
    public string[] Resources { get; }

    public ResourceResolveException(string message, string[] resources) : base(message)
    {
        Resources = resources;
    }

    public ResourceResolveException(string message, string[] resources, Exception? inner) : base(message, inner)
    {
        Resources = resources;
    }

    public string[] GetRecursiveResources()
    {
        List<string> result = new(Resources);

        Exception? current = InnerException;
        while(current != null)
        {
            if(current is ResourceResolveException rre)
            {
                result.AddRange(rre.Resources);
            }

            current = current.InnerException;
        }

        return [.. result];
    }
}
