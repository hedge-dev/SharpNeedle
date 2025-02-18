namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

public class LightIndexMesh : List<LightIndexData>, IBinarySerializable
{
    public MeshSlot Slot { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        int dataCount = reader.Read<int>();
        Capacity = dataCount;
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < dataCount; i++)
            {
                Add(reader.ReadObjectOffset<LightIndexData>());
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Count);
        writer.WriteOffset(() =>
        {
            foreach (LightIndexData mesh in this)
            {
                writer.WriteObjectOffset(mesh);
            }
        });
    }
}

public class LightIndexData : IBinarySerializable
{
    public uint[] LightIndices { get; set; } = [];
    public ushort[] VertexIndices { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        int lightsCount = reader.Read<int>();
        reader.ReadOffset(() => LightIndices = reader.ReadArray<uint>(lightsCount));

        int verticesCount = reader.Read<int>();
        reader.ReadOffset(() => VertexIndices = reader.ReadArray<ushort>(verticesCount));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(LightIndices.Length);
        writer.WriteArrayOffset(LightIndices);

        writer.Write(VertexIndices.Length);
        writer.WriteArrayOffset(VertexIndices);
    }
}