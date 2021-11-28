using System.IO;

namespace SharpNeedle.IO;

public class VirtualFile : IFile
{
    private string mName;

    IDirectory IFile.Parent => Parent;
    public string Path { get; internal set; }
    public VirtualDirectory Parent { get; internal set; }
    public long Length => BaseStream?.Length ?? 0;
    public DateTime LastModified { get; set; }
    public Stream BaseStream { get; set; }

    private bool mLeaveOpen = true;

    public string Name
    {
        get => mName;
        set
        {
            if (mName == value) return;
            Parent?.FileNameChanged(this, mName, value);
            Path = System.IO.Path.Combine(Parent?.Path ?? string.Empty, value);
            mName = value;
        }
    }

    public VirtualFile(string name, Stream baseStream, bool leaveOpen = false)
    {
        Name = name;
        mLeaveOpen = leaveOpen;
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
        if (BaseStream == null)
        {
            BaseStream = new MemoryStream();
            mLeaveOpen = false;
        }

        BaseStream.Position = 0;
        return BaseStream;
    }

    public void Dispose()
    {
        if (!mLeaveOpen) BaseStream.Dispose();
    }

    public override string ToString() => Name;
}