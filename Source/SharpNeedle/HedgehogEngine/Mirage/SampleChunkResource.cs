namespace SharpNeedle.HedgehogEngine.Mirage;
using System.Reflection;
using System.IO;

public abstract class SampleChunkResource : ResourceBase, IBinarySerializable
{
    public uint DataVersion { get; set; }
    public SampleChunkNode Root { get; set; }

    public void SetupNodes()
    {
        NeedleResourceAttribute attribute = GetType().GetCustomAttribute<NeedleResourceAttribute>();
        if(attribute == null || attribute.Type == ResourceType.Raw)
        {
            SetupNodes(GetType().Name);
            return;
        }

        SetupNodes(attribute.Type.ToString());
    }

    public void SetupNodes(string rootName)
    {
        Root = new();

        SampleChunkNode dataRoot = new(rootName);
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

        SampleChunkNode.Flags v2Flags = reader.Read<SampleChunkNode.Flags>();
        reader.Seek(-4, SeekOrigin.Current);

        if(v2Flags.HasFlag(SampleChunkNode.Flags.Root))
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
        DataVersion = reader.Read<uint>();

        uint dataSize = reader.Read<uint>();
        long dataOffset = reader.ReadOffsetValue();

        using(SeekToken token = reader.At())
        {
            // Using a substream to ensure we can't read outside the nodes data
            SubStream dataStream = new(reader.GetBaseStream(), dataOffset, dataSize);
            BinaryObjectReader dataReader = new(dataStream, StreamOwnership.Retain, reader.Endianness);
            Read(dataReader);
        }

        reader.ReadOffsetValue(); // offset table 

        string name = reader.ReadStringOffset();
        if(!string.IsNullOrEmpty(name))
        {
            Name = Path.GetFileNameWithoutExtension(name);
        }
    }

    private void ReadResourceV2(BinaryObjectReader reader)
    {
        long rootStart = reader.Position;
        Root = reader.ReadObject<SampleChunkNode>();

        SampleChunkNode contexts = Root.FindNode("Contexts");
        if(contexts == null)
        {
            return;
        }

        using(SeekToken token = reader.At())
        {
            // Using a substream to ensure we can't read outside the nodes data
            SubStream dataStream = new(reader.GetBaseStream(), contexts.DataOffset, contexts.DataSize);
            BinaryObjectReader dataReader = new(dataStream, StreamOwnership.Retain, reader.Endianness);

            dataReader.OffsetHandler.PushOffsetOrigin(rootStart + 0x10 - contexts.DataOffset);
            Read(dataReader);
            dataReader.PopOffsetOrigin();
        }

        contexts.Data = this;
        DataVersion = contexts.Value;
    }


    public override void Write(IFile file)
    {
        Write(file, Root != null);
    }

    public void Write(IFile file, bool writeNodes)
    {
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);

        if(!writeNodes)
        {
            WriteResourceV1(file, writer);
        }
        else
        {
            if(Root == null)
            {
                SetupNodes();
            }

            WriteResourceV2(writer);
        }
    }

    private void WriteResourceV1(IFile file, BinaryObjectWriter writer)
    {
        SeekToken beginToken = writer.At();

        writer.Write(0); // File Size
        writer.Write(DataVersion);

        SeekToken dataToken = writer.At();
        writer.Write(0); // Data Size
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
            writer.Write((uint)size);
        });

        SeekToken offsetsToken = writer.At();
        writer.Write(0ul);

        writer.Flush();

        SeekToken offTableToken = writer.At();
        offsetsToken.Dispose();
        writer.WriteOffset(() =>
        {
            long[] offsets = writer.OffsetHandler.OffsetPositions.ToArray();
            writer.Write(offsets.Length - 1);
            for(int i = 0; i < offsets.Length - 1; i++)
            {
                writer.Write((uint)(offsets[i] - baseOffset));
            }

            writer.PopOffsetOrigin();
        });

        writer.WriteOffset(() =>
        {
            writer.WriteString(StringBinaryFormat.NullTerminated, file.Name);
            writer.Align(4);
        }, 4);

        // Ensure offset table is written at the end
        offTableToken.Dispose();
        writer.Flush();

        using SeekToken token = writer.At();
        beginToken.Dispose();
        writer.Write((uint)((long)token - (long)beginToken));
    }

    private void WriteResourceV2(BinaryObjectWriter writer)
    {
        SampleChunkNode contexts = Root.FindNode("Contexts");
        if(contexts != null)
            contexts.Value = DataVersion;

        writer.WriteObject(Root);
    }
}