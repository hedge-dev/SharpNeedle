namespace SharpNeedle.Framework.HedgehogEngine.Mirage.TerrainData;

using SharpNeedle.Structs;
using System.Collections.Generic;

public class TerrainGroupSubset : List<string?>, IBinarySerializable
{
    public Sphere Bounds { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        int instanceCount = reader.ReadInt32();
        Clear();
        Capacity = instanceCount;

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < instanceCount; i++)
            {
                Add(reader.ReadStringOffset());
            }
        });

        Bounds = reader.ReadObjectOffset<Sphere>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Count);

        writer.WriteOffset(() =>
        {
            foreach (string? instance in this)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, instance);
            }
        });
        writer.WriteObjectOffset(Bounds);
    }
}
