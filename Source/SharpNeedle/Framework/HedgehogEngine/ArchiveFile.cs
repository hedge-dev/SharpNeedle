namespace SharpNeedle.Framework.HedgehogEngine;

public class ArchiveFile : IFile
{
    IDirectory IFile.Parent => Parent;

    public Archive Parent { get; internal set; }

    public string Name { get; set; }

    public string Path => $"{Parent.Name}/{Name}";

    public MemoryStream? DataStream { get; private set; }

    public DateTime LastModified { get; set; }

    internal long FileOffset { get; set; }

    internal long FileLength { get; set; }

    public long Length => DataStream != null ? DataStream.Length : FileLength;


    public ArchiveFile(Archive parent, string name)
    {
        Parent = parent;
        Name = name;
    }

    public void Dispose()
    {
        DataStream?.Dispose();
    }


    public void EnsureData()
    {
        if (DataStream != null)
        {
            return;
        }

        if (Parent.BaseStream == null)
        {
            throw new InvalidDataException("Parent archvie has no stream");
        }

        using SubStream readStream = new(Parent.BaseStream, FileOffset, FileLength);
        DataStream = new MemoryStream((int)FileLength);
        DataStream.Write(readStream.ReadAllBytes().AsSpan());

        FileOffset = 0;
    }

    public Stream Open(FileAccess access)
    {
        if (access.HasFlag(FileAccess.Write))
        {
            EnsureData();
        }

        if (DataStream != null)
        {
            DataStream.Position = 0;
            return new WrappedStream<MemoryStream>(DataStream);
        }

        if (Parent.BaseStream == null)
        {
            throw new InvalidDataException("Parent archvie has no stream");
        }

        return new SubStream(Parent.BaseStream, FileOffset, FileLength);
    }

    public override string ToString()
    {
        return Name;
    }
}
