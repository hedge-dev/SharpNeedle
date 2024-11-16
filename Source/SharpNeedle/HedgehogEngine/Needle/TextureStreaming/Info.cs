namespace SharpNeedle.HedgehogEngine.Needle.TextureStreaming;

using System.IO;

public class Info : ResourceBase, IBinarySerializable
{
    public string PackageName { get; set; }
    public int Mip4x4Index { get; set; }
    public byte[] Mip4x4 { get; set; }
    public byte[] DdsHeader { get; set; }

    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;

        using Stream stream = file.Open();
        using BinaryObjectReader reader = new(stream, StreamOwnership.Transfer, Endianness.Little);
        Read(reader);
    }

    public override void Write(IFile file)
    {
        using Stream stream = file.Open(FileAccess.Write);
        BinaryObjectWriter writer = new(stream, StreamOwnership.Transfer, Endianness.Little, Encoding.ASCII);
        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        if(reader.ReadInt32() != 0x4953544E)
        {
            throw new InvalidDataException("Invalid signature, expected NTSI");
        }

        if(reader.ReadInt32() != 1)
        {
            throw new InvalidDataException("Invalid version, expected 1");
        }

        if(reader.ReadInt32() != 0)
        {
            throw new InvalidDataException("Invalid reserved value, expected 0");
        }

        int nameSize = reader.ReadInt32();
        int mip4x4Size = reader.ReadInt32();
        Mip4x4Index = reader.ReadInt32();
        PackageName = reader.ReadString(StringBinaryFormat.FixedLength, nameSize);
        Mip4x4 = reader.ReadArray<byte>(mip4x4Size);
        DdsHeader = reader.ReadArray<byte>((int)(reader.Length - reader.Position));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(0x4953544E);
        writer.WriteInt32(1);
        writer.WriteInt32(0);

        writer.WriteInt32(PackageName.Length + 1);
        writer.WriteInt32(Mip4x4.Length);
        writer.WriteInt32(Mip4x4Index);

        writer.WriteString(StringBinaryFormat.NullTerminated, PackageName);
        writer.WriteBytes(Mip4x4);
        writer.WriteBytes(DdsHeader);
    }

    
    public byte[] UnpackDDS(Package package)
    {
        string entryName = Path.GetFileNameWithoutExtension(Name);
        if(!package.TryFindEntry(entryName, out Package.EntryInfo entry))
        {
            throw new InvalidDataException($"Package \"{package.Name}\" has no entry for info \"{entryName}\"");
        }

        MemoryStream result = new();

        result.Write(DdsHeader);

        for(int i = 0; i < entry.MipLevels; i++)
        {
            DataBlock block = package.Blocks[entry.BlockIndex + i];
            result.Write(block.DataStream.ReadAllBytes());
        }

        result.Flush();

        return result.ToArray();
    }
}