namespace SharpNeedle.Resource;

[NeedleResource("internal/raw", ResourceType.Raw)]
public class ResourceRaw : IResource
{
    public IFile? File { get; }
    public string Name { get; set; } = string.Empty;
    public byte[]? Data { get; set; }
    public bool Disposed { get; private set; }


    public ResourceRaw() { }

    public ResourceRaw(IFile file)
    {
        File = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
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


    public void Read(IFile file)
    {
        using Stream stream = file.Open();
        Name = Path.GetFileNameWithoutExtension(file.Name);
        Data = stream.ReadAllBytes();
    }

    public void Write(IFile file)
    {
        if(Data == null)
        {
            throw new InvalidOperationException("Data is null");
        }

        using Stream stream = file.Open(FileAccess.Write);
        stream.Write(Data, 0, Data.Length);
    }


    public void ResolveDependencies(IResourceResolver dir) { }

    public void WriteDependencies(IDirectory dir) { }

    public IEnumerable<ResourceDependency> GetDependencies()
    {
        return [];
    }

}