namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Framework.SurfRide.Draw;
using System.Reflection;

public abstract class SampleChunkResource : ResourceBase, IBinarySerializable
{
    public uint DataVersion { get; set; }
    public SampleChunkNode? Root { get; set; }

    public void SetupNodes()
    {
        NeedleResourceAttribute? attribute = GetType().GetCustomAttribute<NeedleResourceAttribute>();
        if (attribute == null || attribute.Type == ResourceType.Raw)
        {
            SetupNodes(GetType().Name);
            return;
        }

        SetupNodes(attribute.Type.ToString());
    }

    public void SetupNodes(string rootName)
    {
        Root = new()
        {
            Value = SampleChunkNode.RootSignature
        };

        SampleChunkNode dataRoot = new(rootName, 1);
        Root.AddChild(dataRoot);

        dataRoot.AddChild(new("Contexts", this, DataVersion));
    }


    public abstract void Read(BinaryObjectReader reader);
    public abstract void Write(BinaryObjectWriter writer);


    // ReSharper disable AccessToDisposedClosure
    // ReSharper disable once SuspiciousTypeConversion.Global
    public override void Read(IFile file)
    {
        Root = null;
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);

        using BinaryObjectReader reader = new(file.Open(), this is IStreamable ? StreamOwnership.Retain : StreamOwnership.Transfer, Endianness.Big);
        ReadResource(reader);
    }

    public void ReadResource(BinaryObjectReader reader)
    {
        SampleChunkNode.Flags v2Flags = (SampleChunkNode.Flags)reader.ReadUInt32();
        reader.Seek(-4, SeekOrigin.Current);

        if (v2Flags.HasFlag(SampleChunkNode.Flags.Root))
        {
            ReadResourceV2(reader);
        }
        else
        {
            ReadResourceV1(reader);
        }
    }

    private void ReadResourceV1(BinaryObjectReader reader)
    {
        reader.Skip(4); // filesize
        DataVersion = reader.ReadUInt32();

        reader.Skip(4); // data size
        long dataOffset = reader.ReadOffsetValue();

        using (SeekToken token = reader.At())
        {
            reader.Seek(dataOffset, SeekOrigin.Begin);
            reader.OffsetHandler.PushOffsetOrigin(dataOffset);
            Read(reader);
            reader.PopOffsetOrigin();
        }

        reader.ReadOffsetValue(); // offset table 

        string? name = reader.ReadStringOffset();
        if (!string.IsNullOrEmpty(name))
        {
            Name = Path.GetFileNameWithoutExtension(name);
        }
    }

    private void ReadResourceV2(BinaryObjectReader reader)
    {
        long rootStart = reader.Position;
        Root = reader.ReadObject<SampleChunkNode>();

        SampleChunkNode contexts = Root.FindNode("Contexts")
            ?? throw new InvalidDataException("No Contexts node!");

        DataVersion = contexts.Value;

        SampleChunkNode? dataNode = contexts;
        while (dataNode != null && dataNode.DataSize == 0)
        {
            dataNode = dataNode.Parent;
        }

        if (dataNode == null || dataNode == Root)
        {
            throw new InvalidDataException("No data found!");
        }

        SampleChunkNode lastNode = Root;
        while (lastNode.Children.Count != 0)
        {
            lastNode = lastNode.Children[^1];
        }

        using (SeekToken token = reader.At())
        {
            reader.Seek(lastNode.DataOffset, SeekOrigin.Begin);
            reader.OffsetHandler.PushOffsetOrigin(0x10);
            Read(reader);
            reader.PopOffsetOrigin();
        }

        dataNode.Data = this;
    }


    public override void Write(IFile file)
    {
        Write(file, Root != null);
    }

    public void Write(IFile file, bool writeNodes)
    {
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);
        WriteResource(writer, writeNodes, file.Name);
    }

    public void WriteResource(BinaryObjectWriter writer, string filename)
    {
        WriteResource(writer, Root != null, filename);
    }

    public void WriteResource(BinaryObjectWriter writer, bool writeNodes, string filename)
    {
        if (!writeNodes)
        {
            WriteResourceV1(filename, writer);
        }
        else
        {
            if (Root == null)
            {
                SetupNodes();
            }

            WriteResourceV2(writer);
        }
    }

    private void WriteResourceV1(string filename, BinaryObjectWriter writer)
    {
        SeekToken beginToken = writer.At();

        writer.WriteInt32(0); // File Size
        writer.WriteUInt32(DataVersion);

        SeekToken dataToken = writer.At();
        writer.WriteInt32(0); // Data Size
        long baseOffset = 0;

        writer.WriteOffset(() =>
        {
            long start = writer.Position;
            baseOffset = writer.Position;
            writer.PushOffsetOrigin();

            Write(writer);
            writer.Flush();
            writer.Align(writer.DefaultAlignment);
            writer.PopOffsetOrigin();

            long size = writer.Position - start;
            using SeekToken token = writer.At();

            dataToken.Dispose();
            writer.WriteUInt32((uint)size);
        });

        SeekToken offsetsToken = writer.At();
        writer.WriteUInt64(0ul);

        writer.Flush();

        SeekToken offTableToken = writer.At();
        offsetsToken.Dispose();
        writer.WriteOffset(() =>
        {
            long[] offsets = writer.OffsetHandler.OffsetPositions.ToArray();
            writer.WriteInt32(offsets.Length - 1);
            for (int i = 0; i < offsets.Length - 1; i++)
            {
                writer.WriteUInt32((uint)(offsets[i] - baseOffset));
            }

            writer.PopOffsetOrigin();
        });

        writer.WriteOffset(() =>
        {
            writer.WriteString(StringBinaryFormat.NullTerminated, filename);
            writer.Align(4);
        }, 4);

        // Ensure offset table is written at the end
        offTableToken.Dispose();
        writer.Flush();

        using SeekToken token = writer.At();
        beginToken.Dispose();
        writer.WriteUInt32((uint)((long)token - (long)beginToken));
    }

    private void WriteResourceV2(BinaryObjectWriter writer)
    {
        SampleChunkNode? contexts = Root?.FindNode("Contexts");
        if (contexts != null)
        {
            contexts.Value = DataVersion;
        }

        writer.WriteObject(Root);
    }
}