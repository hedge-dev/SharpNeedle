namespace SharpNeedle.Framework.LostWorld.Animation;

public class BlenderData : List<Blender>, IComplexData
{
    public void Read(BinaryObjectReader reader, bool readType)
    {
        if (readType)
        {
            reader.EnsureSignatureNative(1);
        }

        int nodeCount = reader.ReadInt32();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < nodeCount; i++)
            {
                Add(reader.ReadObjectOffset<Blender>());
            }
        });
    }

    public void Write(BinaryObjectWriter writer, bool writeType)
    {
        if (writeType)
        {
            writer.WriteInt32(1);
        }

        writer.WriteInt32(Count);
        writer.WriteOffset(() =>
        {
            foreach (Blender blender in this)
            {
                writer.WriteObjectOffset(blender);
            }
        });
    }
}

public class Blender : IBinarySerializable
{
    public string? Name { get; set; }
    public float Weight { get; set; }
    public List<Node> Nodes { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignatureNative(0);
        Name = reader.ReadStringOffset();
        Weight = reader.ReadSingle();
        Nodes = reader.ReadObject<BinaryList<Node>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(0);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteSingle(Weight);
        writer.WriteObject<BinaryList<Node>>(Nodes);
    }

    public struct Node : IBinarySerializable
    {
        public string? Name;
        public float Weight;
        public int Priority;

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
            Weight = reader.ReadSingle();
            Priority = reader.ReadInt32();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.WriteSingle(Weight);
            writer.WriteInt32(Priority);
        }
    }
}