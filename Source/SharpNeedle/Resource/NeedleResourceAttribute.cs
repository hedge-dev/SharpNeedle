namespace SharpNeedle.Resource;

using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Class)]
public class NeedleResourceAttribute : Attribute
{
    public string Id { get; }

    public ResourceType Type { get; }
    public string? MatchPattern { get; }
    public Type? Owner { get; internal set; }

    internal Func<IFile, bool>? CheckResourceFunc { get; set; }

    public NeedleResourceAttribute(string id)
    {
        Id = id;
    }

    public NeedleResourceAttribute(string id, string matchPattern)
    {
        Id = id;
        MatchPattern = matchPattern;
    }

    public NeedleResourceAttribute(string id, ResourceType type) : this(id)
    {
        Type = type;
    }

    public NeedleResourceAttribute(string id, ResourceType type, string matchPattern) : this(id, type)
    {
        MatchPattern = matchPattern;
    }

    public bool CheckResource(IFile file)
    {
        bool patternMatches = false;
        if (!string.IsNullOrEmpty(MatchPattern))
        {
            patternMatches = Regex.Match(file.Name, MatchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Success;
        }

        if (!patternMatches && CheckResourceFunc != null)
        {
            return CheckResourceFunc(file);
        }

        return patternMatches;
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
    ArchiveInfo
}