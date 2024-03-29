﻿using System.IO;

namespace SharpNeedle.HedgehogEngine.Mirage;

[NeedleResource("hh/light-list", @"\.light-list$")]
public class LightList : SampleChunkResource
{
    public List<string> LightNames { get; set; }
    public List<Light> Lights { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        var count = reader.Read<int>();
        LightNames = new List<string>(count);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
                LightNames.Add(reader.ReadStringOffset());
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        var count = Lights?.Count ?? LightNames.Count;
        writer.Write(count);
        writer.WriteOffset(() =>
        {
            if (Lights != null)
            {
                foreach (var light in Lights)
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, light.Name);
            }
            else
            {
                foreach (var light in LightNames)
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, light);
            }
        });
    }

    public override void WriteDependencies(IDirectory dir)
    {
        if (Lights == null)
            return;

        foreach (var light in Lights)
        {
            using var file = dir.CreateFile($"{light.Name}{Light.Extension}");
            light.Write(file);
            light.WriteDependencies(dir);
        }
    }

    public override IEnumerable<ResourceDependency> GetDependencies()
    {
        if (LightNames?.Count is 0 or null)
            yield break;

        foreach (var name in LightNames)
        {
            yield return new ResourceDependency()
            {
                Name = $"{name}{Light.Extension}",
                Id = Light.ResourceId
            };
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if (LightNames?.Count is 0 or null)
            return;

        Lights = new List<Light>(LightNames.Count);
        foreach (var name in LightNames)
        {
            Lights.Add(resolver.Open<Light>($"{name}{Light.Extension}"));
        }

        LightNames = null;
    }
}