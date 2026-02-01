namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;

using SharpNeedle.Structs;

[NeedleResource("hh/gi-texture-group-info", @"\.gi-texture-group-info$")]
public class GITextureGroupInfo : SampleChunkResource
{
    public List<(string? Name, Sphere Bounds)> Instances { get; set; } = [];
    public List<GITextureGroup> Groups { get; set; } = [];
    public List<int> LowQualityGroups { get; set; } = [];

    public GITextureGroupInfo()
    {
        DataVersion = 2;
    }

    public override void Read(BinaryObjectReader reader)
    {
        int instanceCount = reader.ReadInt32();
        Instances = new(instanceCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < instanceCount; i++)
            {
                Instances.Add((reader.ReadStringOffset(), default));
            }
        });

        reader.ReadOffset(() =>
        {
            Span<(string? Name, Sphere Bounds)> span = CollectionsMarshal.AsSpan(Instances);
            for (int i = 0; i < span.Length; i++)
            {
                span[i].Bounds = reader.ReadObjectOffset<Sphere>();
            }
        });

        int groupCount = reader.ReadInt32();
        Groups = new(groupCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < groupCount; i++)
            {
                Groups.Add(reader.ReadObjectOffset<GITextureGroup>());
            }
        });

        int lowCount = reader.ReadInt32();
        LowQualityGroups = new(lowCount);
        
        reader.ReadOffset(() => reader.ReadCollection(lowCount, LowQualityGroups));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Instances.Count);
        writer.WriteOffset(() =>
        {
            foreach ((string? name, Sphere bounds) in Instances)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, name);
            }
        });

        writer.WriteOffset(() =>
        {
            foreach ((string? name, Sphere bounds) in Instances)
            {
                writer.WriteObjectOffset(bounds);
            }
        });

        writer.WriteInt32(Groups.Count);
        writer.WriteObjectCollectionOffset(Groups);

        writer.WriteInt32(LowQualityGroups.Count);
        writer.WriteCollectionOffset(LowQualityGroups);
    }
}