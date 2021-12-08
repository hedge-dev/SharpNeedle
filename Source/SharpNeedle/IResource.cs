namespace SharpNeedle;

public interface IResource : IDisposable
{
    string Name { get; set; }

    void Read(IFile file);
    void Write(IFile file);

    void ResolveDependencies(IResourceResolver resolver);
    void WriteDependencies(IDirectory dir);
    IEnumerable<ResourceDependency> GetDependencies();
}