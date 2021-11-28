namespace SharpNeedle;
using System.IO;

[BinaryResource("internal/raw", ResourceType.Raw)]
public class ResourceRaw : IResource
{
    public IFile File { get; }
    public string Name { get; set; }
    public byte[] Data { get; set; }

    public ResourceRaw()
    {

    }

    public ResourceRaw(IFile file)
    {
        File = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
    }

    public void Read(IFile file)
    {
        using var stream = file.Open();
        Name = Path.GetFileNameWithoutExtension(file.Name);
        Data = stream.ReadAllBytes();
    }

    public void Write(IFile file)
    {
        using var stream = file.Open(FileAccess.Write);
        stream.Write(Data, 0, Data.Length);
    }

    public void WriteDependencies(IDirectory dir)
    {
        
    }

    public IEnumerable<ResourceDependency> GetDependencies() => Enumerable.Empty<ResourceDependency>();

    public void ResolveDependencies(IDirectory dir)
    {

    }

    public void Dispose()
    {
        File?.Dispose();
    }
}