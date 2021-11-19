﻿namespace SharpNeedle;

public abstract class ResourceBase : IResource
{
    protected bool Disposed { get; private set; }
    public string Name { get; set; }
    public IFile BaseFile { get; protected set; }

    public abstract void Read(IFile file);
    public abstract void Write(IFile file);

    public virtual IEnumerable<ResourceDependency> GetDependencies() => Enumerable.Empty<ResourceDependency>();
    public virtual void ResolveDependencies(IDirectory dir) { }
    public virtual void WriteDependencies(IDirectory dir) {}

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

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            BaseFile?.Dispose();
    }

    public void Dispose()
    {
        if (Disposed)
            return;

        Dispose(true);
        Disposed = true;
        GC.SuppressFinalize(this);
    }

    ~ResourceBase()
    {
        Dispose(false);
    }
}