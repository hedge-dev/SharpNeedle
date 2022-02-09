namespace SharpNeedle.LostWorld;
using BINA;

[NeedleResource("lw/shadow-model", @"\.shadow-model$")]
public class ShadowModel : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("SVLM");
    public List<ShadowMesh> Meshes { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        reader.Skip(4); // Version

        Meshes = reader.ReadObject<BinaryList<ShadowMesh>>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Signature);
        writer.Write(1); // Version

        writer.WriteObject<BinaryList<ShadowMesh>>(Meshes);

        // Unused data, probably means something
        writer.Write(5);
        writer.WriteOffset(() =>
        {
            writer.Write<ushort>(0);
            writer.Write<ushort>(0);
            writer.Write<ushort>(2);
            writer.Write<ushort>(0);

            writer.Write<ushort>(0);
            writer.Write<ushort>(12);
            writer.Write<ushort>(2);
            writer.Write<ushort>(3);

            writer.Write<ushort>(0);
            writer.Write<ushort>(24);
            writer.Write<ushort>(8);
            writer.Write<ushort>(1);

            writer.Write<ushort>(0);
            writer.Write<ushort>(28);
            writer.Write<ushort>(5);
            writer.Write<ushort>(2);

            writer.Write<ushort>(255);
            writer.Write<ushort>(0);
            writer.Write<ushort>(17);
            writer.Write<ushort>(0);
        }, 16);

        writer.Write(0);
    }
}

public class ShadowMesh : IBinarySerializable
{
    public ushort[] Indices { get; set; } = Array.Empty<ushort>();
    public ShadowVertex[] Vertices { get; set; } = Array.Empty<ShadowVertex>();
    public List<ShadowPrimitiveBuffer> Buffers { get; set; } = new();

    public void Read(BinaryObjectReader reader)
    {
        Indices = reader.ReadArrayOffset<ushort>(reader.Read<int>());
        reader.Skip(4);
        Vertices = reader.ReadArrayOffset<ShadowVertex>(reader.Read<int>());
        reader.Skip(4);
        int bufferCount = reader.Read<int>();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < bufferCount; i++)
            {
                var buffer = new ShadowPrimitiveBuffer();
                buffer.Read(reader);
                Buffers.Add(buffer);
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Indices.Length);
        writer.WriteArrayOffset(Indices, 16);
        writer.Write(0);
        writer.Write(Vertices.Length);
        writer.WriteArrayOffset(Vertices, 16);
        writer.Write(0);
        writer.Write(Buffers.Count);
        writer.WriteOffset(() =>
        {
            foreach (var buffer in Buffers)
                buffer.Write(writer);
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
    public byte[] BonePalette { get; set; } = Array.Empty<byte>();

    public void Read(BinaryObjectReader reader)
    {
        PrimitiveType = reader.Read<ShadowPrimitiveType>();
        IndexOffset = reader.ReadInt32();
        IndexCount = reader.ReadInt32();
        VertexOffset = reader.ReadInt32();
        VertexStride = reader.ReadInt32();
        BonePalette = reader.ReadArrayOffset<byte>(reader.Read<int>());
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(PrimitiveType);
        writer.Write(IndexOffset);
        writer.Write(IndexCount);
        writer.Write(VertexOffset);
        writer.Write(VertexStride);
        writer.Write(BonePalette.Length);
        writer.WriteArrayOffset(BonePalette, 16);
    }
}

public struct ShadowVertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public int BlendWeight;
    public int BlendIndices;

    public void AddBlendWeight(int weight, int index)
    {
        for (int i = 0; i < 4; i++)
        {
            int offset = i * 8;
            int mask = 0xFF << offset;

            if ((BlendWeight & mask) != 0)
                continue;

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