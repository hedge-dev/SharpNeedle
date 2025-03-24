namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;
[NeedleResource("hh/light-list", @"\.light-list$")]
public class LightList : SampleChunkResource
{
    public List<ResourceReference<Light>> Lights { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        int count = reader.Read<int>();
        Lights = new(count);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
            {
                Lights.Add(new(reader.ReadStringOffsetOrEmpty()));
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        int count = Lights.Count;
        writer.Write(count);
        writer.WriteOffset(() =>
        {
            foreach (ResourceReference<Light> light in Lights)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, light.Name);
            }
        });
    }

    public override void WriteDependencies(IDirectory dir)
    {
        if (Lights == null)
        {
            return;
        }

        foreach (ResourceReference<Light> light in Lights)
        {
            if (!light.IsValid())
            {
                continue;
            }

            using IFile file = dir.CreateFile($"{light.Name}{Light.Extension}");
            light.Resource!.Write(file);
            light.Resource!.WriteDependencies(dir);
        }
    }

    public override IEnumerable<ResourceDependency> GetDependencies()
    {
        if (Lights.Count == 0)
        {
            yield break;
        }

        foreach (ResourceReference<Light> light in Lights)
        {
            yield return new ResourceDependency()
            {
                Name = $"{light.Name}{Light.Extension}",
                Id = Light.ResourceId
            };
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        List<string> unresolved = [];

        for (int i = 0; i < Lights.Count; i++)
        {
            ResourceReference<Light> light = Lights[i];

            if (light.IsValid())
            {
                continue;
            }

            string filename = $"{light.Name}{Light.Extension}";
            light.Resource = resolver.Open<Light>(filename);

            if (light.Resource == null)
            {
                unresolved.Add(filename);
            }
            else
            {
                Lights[i] = light;
            }
        }

        if (unresolved.Count > 0)
        {
            throw new ResourceResolveException($"Failed to resolve {unresolved.Count} lights!", [.. unresolved]);
        }
    }
}