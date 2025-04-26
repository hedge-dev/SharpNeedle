namespace SharpNeedle.Resource;

using System.Text.Json.Serialization;

public abstract class ResourceBase : IResource
{
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public IFile? BaseFile { get; protected set; }

    protected bool Disposed { get; private set; }

    ~ResourceBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        if (Disposed)
        {
            return;
        }

        Disposed = true;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            BaseFile?.Dispose();
        }
    }


    public abstract void Read(IFile file);

    public abstract void Write(IFile file);

    /// <summary>
    /// Write file to the original location it was read from
    /// </summary>
    public void Save()
    {
        if (this is IStreamable streamable)
        {
            streamable.LoadToMemory();
        }

        if (BaseFile == null)
        {
            throw new IOException("No BaseFile to save to!");
        }

        Write(BaseFile);
        WriteDependencies(BaseFile.Parent);
    }


    public virtual void ResolveDependencies(IResourceResolver dir) { }

    public virtual void WriteDependencies(IDirectory dir) { }

    public virtual IEnumerable<ResourceDependency> GetDependencies()
    {
        return [];
    }

    public override string ToString()
    {
        return Name;
    }
}