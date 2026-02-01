namespace SharpNeedle.Framework.HedgehogEngine;

using SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource(ResourceId, ResourceType.Archive, @"\.ar(\.\d+)?$")]
public class Archive : ResourceBase, IDirectory, IStreamable
{
    public const string ResourceId = "hh/archive";


    public IDirectory? Parent { get; private set; }

    public string Path { get; set; } = string.Empty;

    public IFile? this[string name] => GetFile(Name);

    public int DataAlignment { get; set; } = 16;

    public Stream? BaseStream { get; private set; }

    public List<IFile> Files { get; set; } = new(8);


    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        foreach (IFile file in Files)
        {
            file.Dispose();
        }
    }


    public long CalculateFileSize()
    {
        long size = 0x10L;

        foreach (IFile file in this)
        {
            size = AlignmentHelper.Align(size + 21 + file.Name.Length, DataAlignment) + file.Length;
        }

        return size;
    }

    public PackedFileInfo CalculatePackedInfo()
    {
        long size = 0x10L;
        PackedFileInfo pfi = new();

        foreach (IFile file in this)
        {
            PackedFileInfo.File pfiFile = new();
            size = AlignmentHelper.Align(size + 21 + file.Name.Length, DataAlignment);
            pfiFile.Name = file.Name;
            pfiFile.Offset = (uint)size;
            pfiFile.Size = (uint)file.Length;
            pfi.Files.Add(pfiFile);

            size += file.Length;
        }

        return pfi;
    }


    public void LoadToMemory()
    {
        foreach (IFile file in Files)
        {
            if (file is ArchiveFile aFile)
            {
                aFile.EnsureData();
            }
        }

        BaseFile?.Dispose();
    }

    public override void Read(IFile file)
    {
        Parent = file.Parent;
        Name = file.Name;
        Path = System.IO.Path.Combine(Parent.Path, Name);

        BaseFile = file;
        BaseStream = file.Open();
        BinaryValueReader reader = new(file.Open(), StreamOwnership.Retain, Endianness.Little);
        reader.Skip(0xC);
        DataAlignment = reader.ReadInt32();

        while (reader.Position < reader.Length)
        {
            long baseOffset = reader.Position;

            int dataEnd = reader.ReadInt32();
            uint dataLength = reader.ReadUInt32();
            int dataStart = reader.ReadInt32();
            long lastModifiedBinary = reader.ReadInt64();
            DateTime lastModified = lastModifiedBinary != 0 ? new DateTime(lastModifiedBinary) : DateTime.Now;
            string name = reader.ReadString(StringBinaryFormat.NullTerminated);

            reader.Seek(baseOffset + dataEnd, SeekOrigin.Begin);

            Files.Add(new ArchiveFile(this, name)
            {
                FileOffset = baseOffset + dataStart,
                FileLength = dataLength,
                LastModified = lastModified,
            });
        }
    }

    public override void Write(IFile file)
    {
        using Stream stream = file.Open(FileAccess.Write);
        BinaryObjectWriter writer = new(stream, StreamOwnership.Transfer, Endianness.Little, Encoding.ASCII);

        writer.WriteUInt32(0u);
        writer.WriteUInt32(0x10u);
        writer.WriteUInt32(0x14u);
        writer.WriteInt32(DataAlignment);

        foreach (IFile arFile in this)
        {
            long curPos = writer.Position;
            long dataOffset = AlignmentHelper.Align(curPos + 21 + Encoding.UTF8.GetByteCount(arFile.Name), DataAlignment);
            long blockSize = dataOffset + arFile.Length;

            writer.WriteUInt32((uint)(blockSize - curPos));
            writer.WriteUInt32((uint)arFile.Length);
            writer.WriteUInt32((uint)(dataOffset - curPos));
            writer.WriteInt64(arFile.LastModified.Ticks);
            writer.WriteString(StringBinaryFormat.NullTerminated, arFile.Name);
            writer.Align(DataAlignment);
            arFile.Open().CopyTo(writer.GetBaseStream());
        }
    }


    #region IDictionary method implementations

    public IEnumerable<IDirectory> GetDirectories()
    {
        return [];
    }

    public IDirectory? GetDirectory(string name)
    {
        throw new NotSupportedException();
    }

    public IDirectory CreateDirectory(string name)
    {
        throw new NotSupportedException();
    }

    public bool DeleteDirectory(string name)
    {
        throw new NotSupportedException();
    }


    public IFile? GetFile(string name)
    {
        return Files.FirstOrDefault(x => x.Name == name);
    }

    private void OverwriteDelete(string name, bool overwrite)
    {
        IFile? overwriteFile = Files.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (overwriteFile != null)
        {
            if (overwrite)
            {
                Files.Remove(overwriteFile);
            }
            else
            {
                throw new InvalidOperationException($"Archive \"{Path}\" already has file \"{name}\"!");
            }
        }
    }

    public IFile CreateFile(string name, bool overwrite = true)
    {
        OverwriteDelete(name, overwrite);
        ArchiveFile file = new(this, name);
        Files.Add(file);
        return file;
    }

    public IFile AddFile(IFile file, bool overwrite = true)
    {
        OverwriteDelete(file.Name, overwrite);
        Files.Add(file);
        return file;
    }

    public bool DeleteFile(string name)
    {
        IFile? file = Files.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return file != null && Files.Remove(file);
    }

    #endregion


    public IEnumerator<IFile> GetEnumerator()
    {
        return ((IEnumerable<IFile>)Files).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}