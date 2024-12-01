namespace SharpNeedle.IO;

public class VirtualFile : IFile
{
    private string _mName;

    IDirectory IFile.Parent => Parent;
    public string Path { get; internal set; }
    public VirtualDirectory Parent { get; internal set; }
    public long Length => BaseStream?.Length ?? 0;
    public DateTime LastModified { get; set; }
    public Stream BaseStream { get; set; }

    private bool _mLeaveOpen = true;

    public string Name
    {
        get => _mName;
        set
        {
            if(_mName == value)
            {
                return;
            }

            Parent?.FileNameChanged(this, _mName, value);
            Path = System.IO.Path.Combine(Parent?.Path ?? string.Empty, value);
            _mName = value;
        }
    }

    public VirtualFile(string name, Stream baseStream, bool leaveOpen = false)
    {
        Name = name;
        _mLeaveOpen = leaveOpen;
        BaseStream = baseStream;
    }

    public VirtualFile(string name, VirtualDirectory parent)
    {
        Parent = parent;
        Name = name;
        Path = System.IO.Path.Combine(parent.Path, name);
    }

    public Stream Open(FileAccess access = FileAccess.Read)
    {
        if(BaseStream == null)
        {
            BaseStream = new MemoryStream();
            _mLeaveOpen = false;
        }

        BaseStream.Position = 0;
        return BaseStream;
    }

    public void Dispose()
    {
        if(!_mLeaveOpen)
        {
            BaseStream.Dispose();
        }
    }

    public override string ToString()
    {
        return Name;
    }
}