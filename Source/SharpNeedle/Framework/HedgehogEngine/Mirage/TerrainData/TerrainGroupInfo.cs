namespace SharpNeedle.Framework.HedgehogEngine.Mirage.TerrainData;

using SharpNeedle.Structs;
using System.Collections.Generic;

public class TerrainGroupInfo : IBinarySerializable
{
    public string? Name { get; set; }
    public Sphere Bounds { get; set; }
    public uint MemorySize { get; set; }
    public int SubsetID { get; set; }
    public List<Sphere> Instances { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        Bounds = reader.ReadObjectOffset<Sphere>();
        Name = reader.ReadStringOffset();
        MemorySize = reader.ReadUInt32();

        int instancesCount = reader.ReadInt32();
        Instances = new List<Sphere>(instancesCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < instancesCount; i++)
            {
                Instances.Add(reader.ReadObjectOffset<Sphere>());
            }
        });

        SubsetID = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteObjectOffset(Bounds);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteUInt32(MemorySize);
        writer.WriteInt32(Instances.Count);
        writer.WriteOffset(() =>
        {
            foreach (Sphere instance in Instances)
            {
                writer.WriteObjectOffset(instance);
            }
        });
    }

    public override string ToString()
    {
        return $"{Name}:{SubsetID}";
    }
}
