namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/gi-texture-group-info", @"\.gi-texture-group-info$")]
public class GITextureGroupInfo : SampleChunkResource
{
    public List<(string Name, Sphere Bounds)> Instances { get; set; }
    public List<GITextureGroup> Groups { get; set; }
    public List<int> LowQualityGroups { get; set; }

    public GITextureGroupInfo()
    {
        DataVersion = 2;
    }

    public override void Read(BinaryObjectReader reader)
    {
        reader.Read(out int instanceCount);
        Instances = new (instanceCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < instanceCount; i++)
                Instances.Add(new (reader.ReadStringOffset(), default));
        });

        reader.ReadOffset(() =>
        {
            var span = CollectionsMarshal.AsSpan(Instances);
            for (int i = 0; i < span.Length; i++)
                span[i].Bounds = reader.ReadValueOffset<Sphere>();
        });

        reader.Read(out int groupCount);
        Groups = new (groupCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < groupCount; i++)
                Groups.Add(reader.ReadObjectOffset<GITextureGroup>());
        });

        reader.Read(out int lowCount);
        LowQualityGroups = new (lowCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < lowCount; i++)
                LowQualityGroups.Add(reader.Read<int>());
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Instances.Count);
        writer.WriteOffset(() =>
        {
            foreach (var (name, bounds) in Instances)
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, name);
        });

        writer.WriteOffset(() =>
        {
            foreach (var (name, bounds) in Instances)
                writer.WriteValueOffset(bounds);
        });

        writer.Write(Groups.Count);
        writer.WriteOffset(() =>
        {
            foreach (var group in Groups)
                writer.WriteObjectOffset(group);
        });

        writer.Write(LowQualityGroups.Count);
        writer.WriteOffset(() =>
        {
            foreach (var group in LowQualityGroups)
                writer.Write(group);
        });
    }
}

public class GITextureGroup : IBinarySerializable
{
    public QualityLevel Quality { get; set; }
    public Sphere Bounds { get; set; }
    public List<int> Indices { get; set; }
    public uint MemorySize { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Quality = reader.Read<QualityLevel>();
        reader.Read(out int indexCount);
        Indices = new List<int>(indexCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < indexCount; i++)
                Indices.Add(reader.Read<int>());
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
            foreach (var index in Indices)
                writer.Write(index);
        });
        writer.WriteValueOffset(Bounds);
        writer.Write(MemorySize);
    }

    public enum QualityLevel : int
    {
        High, Medium, Low
    }
}