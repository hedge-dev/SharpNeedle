namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.IO;
using SharpNeedle.Structs;

[NeedleResource("hh/model", ResourceType.Model, @"\.model$")]
public class Model : ModelBase
{
    public List<MorphModel>? Morphs { get; set; }
    public List<Node> Nodes { get; set; } = [];
    public AABB Bounds { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        CommonRead(reader);
        if (DataVersion >= 4)
        {
            Morphs = reader.ReadObject<BinaryList<BinaryPointer<MorphModel, uint>, uint>, uint>(DataVersion).Unwind();
        }

        int nodeCount = reader.ReadInt32();
        Nodes = new List<Node>(nodeCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < nodeCount; i++)
            {
                Nodes.Add(reader.ReadObjectOffset<Node>());
            }
        });

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (reader.Position + 64 >= reader.Length)
                {
                    break;
                }

                Node node = Nodes[i];
                node.Transform = Matrix4x4.Transpose(reader.ReadMatrix4x4());
                Nodes[i] = node;
            }
        });

        if (DataVersion >= 2)
        {
            Bounds = reader.ReadObjectOffset<AABB>();
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        CommonWrite(writer);
        if (DataVersion >= 4)
        {
            writer.WriteInt32(Morphs?.Count ?? 0);
            writer.WriteOffset(() =>
            {
                if (Morphs == null)
                {
                    return;
                }

                foreach (MorphModel morph in Morphs)
                {
                    writer.WriteObjectOffset(morph, DataVersion);
                }
            });
        }

        writer.WriteInt32(Nodes.Count);
        writer.WriteOffset(() =>
        {
            foreach (Node node in Nodes)
            {
                writer.WriteObjectOffset(node);
            }
        });

        writer.WriteOffset(() =>
        {
            foreach (Node node in Nodes)
            {
                writer.WriteMatrix4x4(Matrix4x4.Transpose(node.Transform));
            }
        });

        if (DataVersion >= 2)
        {
            writer.WriteObjectOffset(Bounds);
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        base.ResolveDependencies(resolver);

        if (Morphs != null)
        {
            foreach (MorphModel morph in Morphs)
            {
                morph.ResolveDependencies(resolver);
            }
        }
    }

    public override void WriteDependencies(IDirectory dir)
    {
        base.WriteDependencies(dir);

        if (Morphs != null)
        {
            foreach (MorphModel morph in Morphs)
            {
                morph.WriteDependencies(dir);
            }
        }
    }

    public struct Node : IBinarySerializable
    {
        public string? Name { get; set; }
        public int ParentIndex { get; set; }
        public Matrix4x4 Transform { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            ParentIndex = reader.ReadInt32();
            Name = reader.ReadStringOffset();
            Transform = Matrix4x4.Identity;
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteInt32(ParentIndex);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        }

        public override readonly string ToString()
        {
            return Name ?? base.ToString()!;
        }
    }
}