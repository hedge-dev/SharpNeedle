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
        Quality = (GIQualityLevel)reader.ReadInt32();

        int indexCount = reader.ReadInt32();
        Indices = new List<int>(indexCount);
        reader.ReadOffset(() => reader.ReadCollection(indexCount, Indices));
        
        Bounds = reader.ReadObjectOffset<Sphere>();
        MemorySize = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)Quality);
        writer.WriteInt32(Indices.Count);
        writer.WriteCollectionOffset(Indices);
        writer.WriteObjectOffset(Bounds);
        writer.WriteUInt32(MemorySize);
    }

}
