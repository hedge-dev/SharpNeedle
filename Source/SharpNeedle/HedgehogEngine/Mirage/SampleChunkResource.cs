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
        if (attribute == null)
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
    public abstract void Write(BinaryObjectWriter reader);

    // ReSharper disable AccessToDisposedClosure
    // ReSharper disable once SuspiciousTypeConversion.Global
    public override void Read(IFile file)
    {
        Root = null;
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);

        using var reader = new BinaryObjectReader(file.Open(),
            this is IStreamable ? StreamOwnership.Retain : StreamOwnership.Transfer, Endianness.Big);

        var flags = reader.Read<SampleChunkNode.Flags>();
        if (flags.HasFlag(SampleChunkNode.Flags.Root))
        {
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
        if (!string.IsNullOrEmpty(name))
            Name = name;
    }
    
    public override void Write(IFile file)
    {
        Write(file, Root != null);
    }

    public void Write(IFile file, bool writeNodes)
    {
        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);
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

        {
            writer.WriteOffset(() =>
            {
                var start = writer.Position;
                Write(writer);
                var size = writer.Position - start;
                using var token = writer.At();

                dataToken.Dispose();
                writer.Write((uint)size);
            });

            writer.WriteOffset(() =>
            {
                var offsets = writer.OffsetHandler.OffsetPositions.Skip(1).ToArray();
                writer.Write(offsets.Length);

                foreach (var o in offsets)
                    writer.Write((uint)o);
            }, 4);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, file.Name, alignment: 8);
            writer.Flush();
        }
        using var end = writer.At();

        beginToken.Dispose();
        writer.Write((uint)writer.Length);

        dataToken.Dispose();
    }

    private void WriteResourceV2(IFile file, BinaryObjectWriter writer)
    {
        var contexts = Root.Find("Contexts");
        if (contexts != null)
            contexts.Value = DataVersion;

        writer.WriteObject(Root);
    }
}