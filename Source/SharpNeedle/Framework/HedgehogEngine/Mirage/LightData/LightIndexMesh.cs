namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;

using SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

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