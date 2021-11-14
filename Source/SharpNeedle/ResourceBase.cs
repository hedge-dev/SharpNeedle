namespace SharpNeedle;

public abstract class ResourceBase : IResource
{
    private static readonly IReadOnlyList<ResourceDependency> mEmptyDepends = new List<ResourceDependency>();

    public string Name { get; set; }
    public IFile BaseFile { get; protected set; }

    public abstract void Read(IFile file);
    public abstract void Write(IFile file);

    public virtual IReadOnlyList<ResourceDependency> GetDependencies() => mEmptyDepends;
    public virtual void ResolveDependencies(IDirectory dir) { }

    /// <summary>
    /// Write file to the original location it was read from
    /// </summary>
    public void Save()
    {
        Write(BaseFile);
    }

    public override string ToString()
    {
        return Name;
    }

    public virtual void Dispose()
    {
        BaseFile?.Dispose();
    }
}