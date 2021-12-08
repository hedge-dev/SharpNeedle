namespace SharpNeedle.HedgehogEngine.Mirage;
using System.Reflection;
using System.IO;

public abstract class SampleChunkResource : ResourceBase, IBinarySerializable
{
    protected uint FileSize { get; set; }
    public uint DataVersion { get; set; }
    protected uint DataSize { get; set; }
    public SampleChunkNode Root { get; set; }

    public void SetupNodes()
    {
        var attribute = GetType().GetCustomAttribute<BinaryResourceAttribute>();
        if (attribute == null || attribute.Type == ResourceType.Raw)
        {
            SetupNodes(GetType().Name);
            return;
        }

        SetupNodes(attribute.Type.ToString());
    }

    public void SetupNodes(string rootName)
    {
        Root = new SampleChunkNode();
        Root.Add(rootName).Add("Contexts", DataVersion, this);
    }

    public abstract void Read(BinaryObjectReader reader);
    public abstract void Write(BinaryObjectWriter writer);

    // ReSharper disable AccessToDisposedClosure
    // ReSharper disable once SuspiciousTypeConversion.Global
    public override void Read(IFile file)
    {
        Root = null;
        BaseFile = file;

        using var reader = new BinaryObjectReader(file.Open(),
            this is IStreamable ? StreamOwnership.Retain : StreamOwnership.Transfer, Endianness.Big);

        var flags = reader.Read<SampleChunkNode.Flags>();
        if (flags.HasFlag(SampleChunkNode.Flags.Root))
        {
            Name = Path.GetFileNameWithoutExtension(file.Name);
            reader.Seek(reader.Position - 4, SeekOrigin.Begin);
            Root = reader.ReadObject<SampleChunkNode>();

            FileSize = Root.Size;
            var contexts = Root.Find("Contexts");
            if (contexts == null)
                return;

            contexts.Data = this;
            DataVersion = contexts.Value;
            reader.Seek(contexts.DataOffset, SeekOrigin.Begin);
            reader.OffsetHandler.PushOffsetOrigin(Root.DataOffset);
            Read(reader);
            reader.PopOffsetOrigin();
            return;
        }

        FileSize = (uint)flags;
        DataVersion = reader.Read<uint>();
        DataSize = reader.Read<uint>();

        reader.ReadOffset(() =>
        {
            reader.PushOffsetOrigin();
                
            Read(reader);

            reader.PopOffsetOrigin();
        });

        reader.ReadOffsetValue();

        var name = reader.ReadStringOffset();
        if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(Name))
            Name = name;
        else
            Name = string.IsNullOrEmpty(Name) ? Path.GetFileNameWithoutExtension(file.Name) : Name;
    }
    
    public override void Write(IFile file)
    {
        Write(file, Root != null);
    }

    public void Write(IFile file, bool writeNodes)
    {
        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Retain, Endianness.Big);
        if (!writeNodes)
            WriteResourceV1(file, writer);
        else
        {
            if (Root == null)
                SetupNodes();

            WriteResourceV2(file, writer);
        }
    }

    private void WriteResourceV1(IFile file, BinaryObjectWriter writer)
    {
        var beginToken = writer.At();

        writer.Write(0); // File Size
        writer.Write(DataVersion);

        var dataToken = writer.At();
        writer.Write(0); // Data Size
        long baseOffset = 0;

        writer.WriteOffset(() =>
        {
            var start = writer.Position;
            baseOffset = writer.Position;
            writer.PushOffsetOrigin();

            Write(writer);
            writer.Flush();
            writer.Align(writer.DefaultAlignment);
            writer.PopOffsetOrigin();

            var size = writer.Position - start;
            using var token = writer.At();

            dataToken.Dispose();
            writer.Write((uint)size);
        });

        var offsetsToken = writer.At();
        writer.Write(0ul);

        writer.Flush();

        var offTableToken = writer.At();
        offsetsToken.Dispose();
        writer.WriteOffset(() =>
        {
            var offsets = writer.OffsetHandler.OffsetPositions.ToArray();
            writer.Write(offsets.Length - 1);
            for (int i = 0; i < offsets.Length - 1; i++)
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

        using var token = writer.At();
        beginToken.Dispose();
        writer.Write((uint)((long)token - (long)beginToken));
    }

    private void WriteResourceV2(IFile file, BinaryObjectWriter writer)
    {
        var contexts = Root.Find("Contexts");
        if (contexts != null)
            contexts.Value = DataVersion;

        writer.WriteObject(Root);
    }
}