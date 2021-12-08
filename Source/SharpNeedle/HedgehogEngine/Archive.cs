namespace SharpNeedle.HedgehogEngine;

using System.IO;

[BinaryResource(ResourceId, ResourceType.Archive, @"\.ar(\.\d+)?$")]
public class Archive : ResourceBase, IDirectory, IStreamable
{
    public const string ResourceId = "hh/archive";

    public List<IFile> Files { get; set; } = new(8);

    public IDirectory Parent { get; private set; }

    public string Path { get; set; }
    public IFile this[string name] => GetFile(Name);

    public bool DeleteFile(string name)
    {
        var file = Files.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (file == null)
            return false;

        return Files.Remove(file);
    }

    // Format doesn't support directories
    public bool DeleteDirectory(string name) => false;
    public IDirectory OpenDirectory(string name) => null;
    public IDirectory CreateDirectory(string name) => null;

    public IFile CreateFile(string name)
    {
        var file = new ArchiveFile(this)
        {
            Name = name
        };

        return file;
    }

    public IFile Add(IFile file)
    {
        DeleteFile(file.Name);
        Files.Add(file);
        return file;
    }

    public IEnumerable<IDirectory> GetDirectories() => Enumerable.Empty<IDirectory>();

    public IFile GetFile(string name)
        => Files.FirstOrDefault(x => x.Name == name);

    public Stream BaseStream { get; private set; }

    public override void Read(IFile file)
    {
        Parent = file.Parent;
        Name = file.Name;
        Path = System.IO.Path.Combine(Parent.Path, Name);

        BaseFile = file;
        BaseStream = file.Open();
        var reader = new BinaryValueReader(file.Open(), StreamOwnership.Retain, Endianness.Little);
        reader.Skip(0x10);

        while (reader.Position < reader.Length)
        {
            var baseOffset = reader.Position;

            var dataEnd = reader.Read<int>();
            var dataLength = reader.Read<uint>();
            var dataStart = reader.Read<int>();
            var lastModifiedBinary = reader.Read<long>();
            var lastModified = lastModifiedBinary != 0 ? new DateTime(lastModifiedBinary) : DateTime.Now;
            var name = reader.ReadString(StringBinaryFormat.NullTerminated);

            reader.Seek(baseOffset + dataEnd, SeekOrigin.Begin);

            Files.Add(new ArchiveFile(this)
            {
                FileOffset = baseOffset + dataStart,
                FileLength = dataLength,
                Name = name,
                LastModified = lastModified,
            });
        }
    }

    public override void Write(IFile file)
    {
        if (file.Equals(BaseFile))
            LoadToMemory();

        using var stream = file.Open(FileAccess.Write);
        var writer = new BinaryObjectWriter(stream, StreamOwnership.Transfer, Endianness.Little, Encoding.ASCII);

        writer.Write(0u);
        writer.Write(0x10u);
        writer.Write(0x14u);
        writer.Write(0x10u);

        foreach (var arFile in this)
        {
            writer.PushOffsetOrigin();

            var startOffset = 0L;
            var endToken = writer.At(0, SeekOrigin.Current);
            writer.Write(0);
            writer.Write((uint)arFile.Length);
            writer.WriteOffset(() =>
            {
                startOffset = writer.OffsetHandler.ResolveOffset(1) - 1;
                using var readStream = arFile.Open();
                readStream.CopyTo(writer.GetBaseStream());
            });
                
            writer.Write(arFile.LastModified.Ticks);
            writer.WriteString(StringBinaryFormat.NullTerminated, arFile.Name);
            writer.Align(0x10);

            writer.Flush();

            using var nextToken = writer.At(0, SeekOrigin.Current);

            // Write end offset
            endToken.Dispose();
            writer.Write((uint)(startOffset + arFile.Length));

            writer.PopOffsetOrigin();
        }
    }

    public void LoadToMemory()
    {
        foreach (var file in Files)
        {
            if (file is ArchiveFile aFile)
                aFile.EnsureData();
        }

        BaseFile?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var file in Files)
            file.Dispose();
    }

    public IEnumerator<IFile> GetEnumerator()
        => ((IEnumerable<IFile>)Files).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public class ArchiveFile : IFile
{
    IDirectory IFile.Parent => Parent;
    public Archive Parent { get; internal set; }
    public string Name { get; set; }
    public string Path => $"{Parent.Name}/{Name}";
    public MemoryStream DataStream { get; private set; }
    public DateTime LastModified { get; set; }

    internal long FileOffset { get; set; }
    internal long FileLength { get; set; }

    public long Length
    {
        get
        {
            if (DataStream != null)
                return DataStream.Length;

            return FileLength;
        }
    }


    public ArchiveFile(Archive parent)
    {
        Parent = parent;
    }

    public void EnsureData()
    {
        if (FileOffset == 0)
            return;

        using var readStream = new SubStream(Parent.BaseStream, FileOffset, FileLength);
        DataStream = new MemoryStream(readStream.ReadAllBytes(), true);

        FileOffset = 0;
    }

    public Stream Open(FileAccess access)
    {
        if (access.HasFlag(FileAccess.Write))
            EnsureData();

        if (DataStream != null)
            return DataStream;

        return new SubStream(Parent.BaseStream, FileOffset, FileLength);
    }

    public override string ToString() => Name;

    public void Dispose()
    {
        DataStream?.Dispose();
    }
}