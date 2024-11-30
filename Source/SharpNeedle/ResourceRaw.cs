namespace SharpNeedle;
using System.IO;

[NeedleResource("internal/raw", ResourceType.Raw)]
public class ResourceRaw : IResource
{
    public IFile File { get; }
    public string Name { get; set; }
    public byte[] Data { get; set; }
    public bool Disposed { get; private set; }

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
        using Stream stream = file.Open();
        Name = Path.GetFileNameWithoutExtension(file.Name);
        Data = stream.ReadAllBytes();
    }

    public void Write(IFile file)
    {
        using Stream stream = file.Open(FileAccess.Write);
        stream.Write(Data, 0, Data.Length);
    }

    public void WriteDependencies(IDirectory dir)
    {

    }

    public IEnumerable<ResourceDependency> GetDependencies()
    {
        return [];
    }

    public void ResolveDependencies(IResourceResolver dir)
    {

    }

    public void Dispose()
    {
        if(Disposed)
        {
            return;
        }

        Disposed = true;
        Data = null;
        File?.Dispose();
        GC.SuppressFinalize(this);
        ResourceManager.Instance.Close(this);
    }
}