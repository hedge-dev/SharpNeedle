namespace SharpNeedle.IO;

public class VirtualFile : IFile
{
    private string _name = string.Empty;
    private bool _leaveOpen = true;


    IDirectory IFile.Parent => Parent;


    /// <summary>
    /// Directory containing this file.
    /// </summary>
    public VirtualDirectory Parent { get; internal set; }

    public string Path { get; internal set; }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value)
            {
                return;
            }

            Parent?.FileNameChanged(this, _name, value);
            Path = System.IO.Path.Combine(Parent?.Path ?? string.Empty, value);
            _name = value;
        }
    }

    public long Length => BaseStream?.Length ?? 0;

    public DateTime LastModified { get; set; }

    public Stream? BaseStream { get; set; }


    public VirtualFile(string name, VirtualDirectory parent)
    {
        Parent = parent;
        Name = name;
        Path = System.IO.Path.Combine(parent.Path, name);
    }


    public Stream Open(FileAccess access = FileAccess.Read)
    {
        if (BaseStream == null)
        {
            BaseStream = new MemoryStream();
            _leaveOpen = false;
        }

        BaseStream.Position = 0;
        return BaseStream;
    }

    public void Dispose()
    {
        if (!_leaveOpen)
        {
            BaseStream?.Dispose();
        }
    }

    public override string ToString()
    {
        return Name;
    }
}