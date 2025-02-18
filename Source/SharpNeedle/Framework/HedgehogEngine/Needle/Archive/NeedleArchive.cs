namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using SharpNeedle.IO;
using System.Collections.Generic;

[NeedleResource("hh/needle/nedarc")]
public class NeedleArchive : ResourceBase
{
    public const string Signature = "NEDARC";

    private byte _version = 1;

    public byte Version
    {
        get => _version;
        set
        {
            if (value != 1)
            {
                throw new ArgumentException($"Invalid archive version {value}, must be 1!");
            }

            _version = value;
        }
    }

    public string Type { get; set; } = "arc";

    public NeedleArchiveDataOffsetMode OffsetMode { get; set; }

    public List<NeedleArchiveBlock> DataBlocks { get; set; } = [];


    public override void Read(IFile file)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Big);

        string signature = reader.ReadString(StringBinaryFormat.FixedLength, 6);
        if (signature != Signature)
        {
            throw new InvalidDataException($"Needle archive expected with a signature of \"{Signature}\", but had \"{signature}\" instead!");
        }

        byte versionCharacter = reader.ReadByte();
        int version = reader.ReadByte() - (byte)'0';
        if (versionCharacter != (byte)'V' || version < 0 || version > 9)
        {
            throw new InvalidDataException($"Needle archive block does not have a valid version in its signature!");
        }

        Version = (byte)version;

        uint archiveSize = reader.ReadUInt32();
        long archiveEnd = reader.Position - 4 + archiveSize;

        Type = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);

        while (reader.Position < archiveEnd)
        {
            DataBlocks.Add(NeedleArchiveBlock.ReadArchiveBlock(reader, file.Name, OffsetMode));
        }
    }

    public override void Write(IFile file)
    {
        BaseFile = file;
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);

        writer.WriteString(StringBinaryFormat.FixedLength, Signature, 6);
        writer.WriteByte((byte)'V');
        writer.WriteByte((byte)(Version + '0'));

        SeekToken sizeToken = writer.At();
        long dataStartPos = writer.Position;
        writer.WriteInt32(0);

        writer.WriteString(StringBinaryFormat.NullTerminated, Type);
        writer.Align(4);

        foreach (NeedleArchiveBlock block in DataBlocks)
        {
            block.WriteBlock(writer, file.Name, OffsetMode);
        }

        long dataEndPos = writer.Position;

        using (SeekToken temp = writer.At())
        {
            sizeToken.Dispose();
            writer.WriteInt32((int)(dataEndPos - dataStartPos));
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        foreach (NeedleArchiveBlock block in DataBlocks)
        {
            block.ResolveDependencies(resolver);
        }
    }
}
