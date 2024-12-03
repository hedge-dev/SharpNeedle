namespace SharpNeedle.Framework.LostWorld.Animation;

public class BlenderData : List<Blender>, IComplexData
{
    public void Read(BinaryObjectReader reader, bool readType)
    {
        if(readType)
        {
            reader.EnsureSignatureNative(1);
        }

        int nodeCount = reader.Read<int>();
        reader.ReadOffset(() =>
        {
            for(int i = 0; i < nodeCount; i++)
            {
                Add(reader.ReadObjectOffset<Blender>());
            }
        });
    }

    public void Write(BinaryObjectWriter writer, bool writeType)
    {
        if(writeType)
        {
            writer.Write(1);
        }

        writer.Write(Count);
        writer.WriteOffset(() =>
        {
            foreach(Blender blender in this)
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
        Weight = reader.Read<float>();
        Nodes = reader.ReadObject<BinaryList<Node>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(0);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Weight);
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
            Weight = reader.Read<float>();
            Priority = reader.Read<int>();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(Weight);
            writer.Write(Priority);
        }
    }
}