namespace SharpNeedle.Resource;
using System;

public class ResourceResolveException : Exception
{
    public string[] Resources { get; }

    public ResourceResolveException(string message, string[] resources) : base(message)
    {
        Resources = resources;
    }

    public ResourceResolveException(string message, string[] resources, Exception inner) : base(message, inner is ResourceResolveException ? null :inner)
    {
        if(inner is ResourceResolveException innerRes)
        {
            Resources = [.. resources, .. innerRes.Resources];
        }
        else
        {
            Resources = resources;
        }
    }
}
