namespace SharpNeedle.Studio.Models;

using SharpNeedle.Resource;

[AttributeUsage(AttributeTargets.Class)]
public class ResourceEditorAttribute : Attribute
{
    public Type Type { get; }
    public TypeMatchMethod MatchMethod { get; }
        
    internal Func<IResource, IViewModel> EditorCreator { get; set; }

    public ResourceEditorAttribute(Type type)
    {
        Type = type;
    }

    public ResourceEditorAttribute(Type type, TypeMatchMethod method) : this(type)
    {
        MatchMethod = method;
    }

    public IViewModel CreateEditor(IResource res)
    {
        return EditorCreator?.Invoke(res);
    }

    public bool Match(Type type)
    {
        switch (MatchMethod)
        {
            case TypeMatchMethod.Assignable:
                return Type.IsAssignableFrom(type);

            case TypeMatchMethod.Equals:
                return type == Type;
        }

        return false;
    }
}

public enum TypeMatchMethod
{
    Equals,
    Assignable
}

[AttributeUsage(AttributeTargets.Method)]
public class ResourceEditorCreatorAttribute : Attribute
{

}