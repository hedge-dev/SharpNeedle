namespace SharpNeedle;

[AttributeUsage(AttributeTargets.Class)]
public class BinaryResourceAttribute : Attribute
{
    public string Id { get; }
    public ResourceType Type { get; }
    public Type Owner { get; internal set; }

    internal Func<IFile, bool> CheckResourceFunc { get; set; }

    public BinaryResourceAttribute(string id)
    {
        Id = id;
    }

    public BinaryResourceAttribute(string id, ResourceType type) : this(id)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class ResourceCheckFunctionAttribute : Attribute
{

}

public enum ResourceType
{
    Raw,
    Model,
    Animation,
    Material,
    Light,
    Archive,
}