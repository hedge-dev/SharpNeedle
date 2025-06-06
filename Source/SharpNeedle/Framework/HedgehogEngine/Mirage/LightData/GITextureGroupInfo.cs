﻿namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;

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
        reader.Read(out int instanceCount);
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
                span[i].Bounds = reader.ReadValueOffset<Sphere>();
            }
        });

        reader.Read(out int groupCount);
        Groups = new(groupCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < groupCount; i++)
            {
                Groups.Add(reader.ReadObjectOffset<GITextureGroup>());
            }
        });

        reader.Read(out int lowCount);
        LowQualityGroups = new(lowCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < lowCount; i++)
            {
                LowQualityGroups.Add(reader.Read<int>());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Instances.Count);
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
                writer.WriteValueOffset(bounds);
            }
        });

        writer.Write(Groups.Count);
        writer.WriteOffset(() =>
        {
            foreach (GITextureGroup group in Groups)
            {
                writer.WriteObjectOffset(group);
            }
        });

        writer.Write(LowQualityGroups.Count);
        writer.WriteOffset(() =>
        {
            foreach (int group in LowQualityGroups)
            {
                writer.Write(group);
            }
        });
    }
}