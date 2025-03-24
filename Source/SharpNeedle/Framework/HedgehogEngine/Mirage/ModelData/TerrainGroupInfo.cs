namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TerrainGroupInfo : IBinarySerializable
{
    public string? Name { get; set; }
    public Sphere Bounds { get; set; }
    public uint MemorySize { get; set; }
    public int SubsetID { get; set; }
    public List<Sphere> Instances { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        Bounds = reader.ReadValueOffset<Sphere>();
        Name = reader.ReadStringOffset();
        MemorySize = reader.Read<uint>();

        reader.Read(out int instancesCount);
        Instances = new List<Sphere>(instancesCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < instancesCount; i++)
            {
                Instances.Add(reader.ReadValueOffset<Sphere>());
            }
        });

        SubsetID = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteValueOffset(Bounds);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(MemorySize);
        writer.Write(Instances.Count);
        writer.WriteOffset(() =>
        {
            foreach (Sphere instance in Instances)
            {
                writer.WriteValueOffset(instance);
            }
        });
    }

    public override string ToString()
    {
        return $"{Name}:{SubsetID}";
    }
}
