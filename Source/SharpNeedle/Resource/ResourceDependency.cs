namespace SharpNeedle.Resource;

public record ResourceDependency
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Name}:{Id}";
    }
}