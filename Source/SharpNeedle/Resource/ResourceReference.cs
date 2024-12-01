namespace SharpNeedle.Resource;

public struct ResourceReference<TResource> where TResource : IResource
{
    private string _name = string.Empty;

    public string Name
    {
        readonly get => Resource?.Name ?? _name;
        set
        {
            Resource = default;
            _name = value;
        }
    }

    public TResource? Resource { get; set; }

    public ResourceReference()
    {
        Resource = default;
    }

    public ResourceReference(string name) : this()
    {
        Name = name;
    }

    public ResourceReference(in TResource resource) : this()
    {
        Resource = resource;
    }


    public readonly bool IsValid()
    {
        return Resource != null;
    }

    public override string ToString()
    {
        return Resource?.ToString() ?? Name;
    }

    public static implicit operator ResourceReference<TResource>(string name)
    {
        return new(name);
    }

    public static implicit operator ResourceReference<TResource>(in TResource resource)
    {
        return new(resource);
    }
}