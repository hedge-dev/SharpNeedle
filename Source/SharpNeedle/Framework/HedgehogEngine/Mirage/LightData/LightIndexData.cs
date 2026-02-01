namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;

public class LightIndexData : IBinarySerializable
{
    public uint[] LightIndices { get; set; } = [];
    public ushort[] VertexIndices { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        int lightsCount = reader.ReadInt32();
        LightIndices = reader.ReadArrayOffset<uint>(lightsCount);

        int verticesCount = reader.ReadInt32();
        VertexIndices = reader.ReadArrayOffset<ushort>(verticesCount);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(LightIndices.Length);
        writer.WriteArrayOffset(LightIndices);

        writer.WriteInt32(VertexIndices.Length);
        writer.WriteArrayOffset(VertexIndices);
    }
}
