namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;

using SharpNeedle.Structs;
using System.Collections.Generic;

public class GITextureGroup : IBinarySerializable
{
    public GIQualityLevel Quality { get; set; }
    public Sphere Bounds { get; set; }
    public List<int> Indices { get; set; } = [];
    public uint MemorySize { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Quality = reader.Read<GIQualityLevel>();
        reader.Read(out int indexCount);
        Indices = new List<int>(indexCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < indexCount; i++)
            {
                Indices.Add(reader.Read<int>());
            }
        });

        Bounds = reader.ReadValueOffset<Sphere>();
        MemorySize = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Quality);
        writer.Write(Indices.Count);
        writer.WriteOffset(() =>
        {
            foreach (int index in Indices)
            {
                writer.Write(index);
            }
        });
        writer.WriteValueOffset(Bounds);
        writer.Write(MemorySize);
    }

}
