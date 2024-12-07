namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using SharpNeedle.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class NeedleArchiveBlock : ResourceBase
{
    private byte _version;

    public abstract string Signature { get; }

    public byte Version
    {
        get => _version;
        set
        {
            if(value > 9)
            {
                throw new ArgumentException("Needle archive block version cannot be greater than 9!");
            }

            if(!IsVersionValid(value))
            {
                throw new ArgumentException("Needle archive block version invalid!");
            }

            _version = value;
        }
    }

    public string Type { get; set; }

    protected NeedleArchiveBlock(string type)
    {
        Type = type;
    }


    protected abstract bool IsVersionValid(int version);


    public override void Read(IFile file)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Big);
        ReadBlockData(reader, file.Name, NeedleArchvieDataOffsetMode.Default);
    }

    public void ReadBlock(BinaryObjectReader reader, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        // this occurs *after* the first 6 signature bytes have been read

        byte versionCharacter = reader.ReadByte();
        int version = reader.ReadByte() - (byte)'0';
        if(versionCharacter != (byte)'V' || version < 0 || version > 9)
        {
            throw new InvalidDataException($"Needle archive block does not have a valid version in its signature!");
        }

        Version = (byte)version;
        Type = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);

        int size = reader.ReadInt32();
        using(SeekToken dataStart = reader.At())
        {
            using SubStream dataStream = new(reader.GetBaseStream(), reader.Position, size);
            using BinaryObjectReader dataReader = new(dataStream, StreamOwnership.Retain, Endianness.Big);
            ReadBlockData(dataReader, filename, offsetMode);
        }

        reader.Skip(size);
    }

    protected abstract void ReadBlockData(BinaryObjectReader reader, string filename, NeedleArchvieDataOffsetMode offsetMode);

    public static NeedleArchiveBlock ReadArchiveBlock(BinaryObjectReader reader, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        string signature = reader.ReadString(StringBinaryFormat.FixedLength, 6);

        NeedleArchiveBlock result = signature switch
        {
            LODInfoBlock.LODInfoSignature => new LODInfoBlock(),
            ModelBlock.ModelSignature => new ModelBlock(),
            _ => new UnknownBlock(signature),
        };

        result.ReadBlock(reader, filename, offsetMode);
        return result;
    }


    public override void Write(IFile file)
    {
        BaseFile = file;
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);
        WriteBlockData(writer, file.Name, NeedleArchvieDataOffsetMode.Default);
    }

    public void WriteBlock(BinaryObjectWriter writer, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        if(Version > 9)
        {
            throw new InvalidOperationException($"Version invalid! Must be <= 9, is {Version}");
        }

        writer.WriteString(StringBinaryFormat.FixedLength, Signature, 6);
        writer.WriteByte((byte)'V');
        writer.WriteByte((byte)(Version + (byte)'0'));
        writer.WriteString(StringBinaryFormat.NullTerminated, Type);

        SeekToken sizeToken = writer.At();
        writer.WriteInt32(0);

        long dataStartPos = writer.Position;
        WriteBlockData(writer, filename, offsetMode);
        writer.Align(4);

        long dataEndPos = writer.Position;

        using(SeekToken temp = writer.At())
        {
            sizeToken.Dispose();
            writer.WriteInt32((int)(dataEndPos - dataStartPos));
        }
    }

    protected abstract void WriteBlockData(BinaryObjectWriter writer, string filename, NeedleArchvieDataOffsetMode offsetMode);

    public override string ToString()
    {
        return Type;
    }
}
