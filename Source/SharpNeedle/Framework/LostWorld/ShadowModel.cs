namespace SharpNeedle.Framework.LostWorld;

using SharpNeedle.Framework.BINA;

[NeedleResource("lw/shadow-model", @"\.shadow-model$")]
public class ShadowModel : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("SVLM");

    private static readonly ushort[] _unusedData = [
        0, 0, 2, 0,
        0, 12, 2, 3,
        0, 24, 8, 1,
        0, 28, 5, 2,
        255, 0, 17, 0,
    ];

    public List<ShadowMesh> Meshes { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        reader.Skip(4); // Version

        Meshes = reader.ReadObject<BinaryList<ShadowMesh>>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Signature);
        writer.WriteInt32(1); // Version

        writer.WriteObject<BinaryList<ShadowMesh>>(Meshes);

        // Unused data, probably means something
        writer.WriteInt32(_unusedData.Length / 4);
        writer.WriteArrayOffset(_unusedData, 16);

        writer.WriteInt32(0);
    }
}

public class ShadowMesh : IBinarySerializable
{
    public ushort[] Indices { get; set; } = [];
    public ShadowVertex[] Vertices { get; set; } = [];
    public List<ShadowPrimitiveBuffer> Buffers { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        Indices = reader.ReadArrayOffset<ushort>(reader.ReadInt32());
        reader.Skip(4);
        Vertices = reader.ReadObjectArrayOffset<ShadowVertex>(reader.ReadInt32());
        reader.Skip(4);
        int bufferCount = reader.ReadInt32();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < bufferCount; i++)
            {
                ShadowPrimitiveBuffer buffer = new();
                buffer.Read(reader);
                Buffers.Add(buffer);
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Indices.Length);
        writer.WriteArrayOffset(Indices, 16);
        writer.WriteInt32(0);
        writer.WriteInt32(Vertices.Length);
        writer.WriteOffset(() => writer.WriteObjectCollection(Vertices), 16);
        writer.WriteInt32(0);
        writer.WriteInt32(Buffers.Count);
        writer.WriteOffset(() =>
        {
            foreach (ShadowPrimitiveBuffer buffer in Buffers)
            {
                buffer.Write(writer);
            }
        }, 4);
    }
}

public class ShadowPrimitiveBuffer : IBinarySerializable
{
    public ShadowPrimitiveType PrimitiveType { get; set; }
    public int IndexOffset { get; set; }
    public int IndexCount { get; set; }
    public int VertexOffset { get; set; }
    public int VertexStride { get; set; }
    public byte[] BonePalette { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        PrimitiveType = (ShadowPrimitiveType)reader.ReadInt32();
        IndexOffset = reader.ReadInt32();
        IndexCount = reader.ReadInt32();
        VertexOffset = reader.ReadInt32();
        VertexStride = reader.ReadInt32();
        BonePalette = reader.ReadArrayOffset<byte>(reader.ReadInt32());
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)PrimitiveType);
        writer.WriteInt32(IndexOffset);
        writer.WriteInt32(IndexCount);
        writer.WriteInt32(VertexOffset);
        writer.WriteInt32(VertexStride);
        writer.WriteInt32(BonePalette.Length);
        writer.WriteArrayOffset(BonePalette, 16);
    }
}

public struct ShadowVertex : IBinarySerializable
{
    public Vector3 Position;
    public Vector3 Normal;
    public int BlendWeight;
    public int BlendIndices;

    public void Read(BinaryObjectReader reader)
    {
        Position = reader.ReadVector3();
        Normal = reader.ReadVector3();
        BlendWeight = reader.ReadInt32();
        BlendIndices = reader.ReadInt32();
    }

    public readonly void Write(BinaryObjectWriter writer)
    {
        writer.WriteVector3(Position);
        writer.WriteVector3(Normal);
        writer.WriteInt32(BlendWeight);
        writer.WriteInt32(BlendIndices);
    }

    public void AddBlendWeight(int weight, int index)
    {
        for (int i = 0; i < 4; i++)
        {
            int offset = i * 8;
            int mask = 0xFF << offset;

            if ((BlendWeight & mask) != 0)
            {
                continue;
            }

            BlendWeight = (BlendWeight & ~mask) | (weight << offset);
            BlendIndices = (BlendIndices & ~mask) | (index << offset);

            break;
        }
    }

    public void NormalizeBlendWeight()
    {
        int b1 = BlendWeight & 0xFF;
        int b2 = (BlendWeight >> 8) & 0xFF;
        int b3 = (BlendWeight >> 16) & 0xFF;
        int b4 = (BlendWeight >> 24) & 0xFF;

        BlendWeight += 0xFF - (b1 + b2 + b3 + b4);
    }
}

public enum ShadowPrimitiveType : int
{
    Points = 1,
    Lines = 2,
    LineStrip = 3,
    Triangles = 4,
    TriangleFan = 5,
    TriangleStrip = 6,
    Quads = 19,
    QuadStrip = 20
}