namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/model", ResourceType.Model, @"\.model$")]
public class Model : ModelBase
{
    public List<MorphModel> Morphs { get; set; }
    public List<Node> Nodes { get; set; }
    public AABB Bounds { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        CommonRead(reader);
        if (DataVersion >= 4)
            Morphs = reader.ReadObject<BinaryList<BinaryPointer<MorphModel>>>().Unwind();
        
        reader.Read(out int nodeCount);
        Nodes = new List<Node>(nodeCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < nodeCount; i++)
                Nodes.Add(reader.ReadObjectOffset<Node>());
        });

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                node.Transform = reader.Read<Matrix4x4>();
                Nodes[i] = node;
            }
        });

        if (DataVersion >= 2)
            Bounds = reader.ReadObjectOffset<AABB>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        CommonWrite(writer);
        if (DataVersion >= 4)
        {
            writer.Write(Morphs.Count);
            writer.WriteOffset(() =>
            {
                foreach (var morph in Morphs)
                    writer.WriteObjectOffset(morph);
            });
        }

        writer.Write(Nodes.Count);
        writer.WriteOffset(() =>
        {
            foreach (var node in Nodes)
                writer.WriteObjectOffset(node);
        });

        writer.WriteOffset(() =>
        {
            foreach (var node in Nodes)
                writer.Write(node.Transform);
        });

        if (DataVersion >= 2)
            writer.WriteObjectOffset(Bounds);
    }

    public struct Node : IBinarySerializable
    {
        public string Name;
        public int ParentIndex;
        public Matrix4x4 Transform;

        public void Read(BinaryObjectReader reader)
        {
            ParentIndex = reader.Read<int>();
            Name = reader.ReadStringOffset();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(ParentIndex);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}