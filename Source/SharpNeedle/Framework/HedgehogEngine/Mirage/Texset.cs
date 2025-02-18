namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource("hh/texset", @"\.texset$")]
public class Texset : SampleChunkResource
{
    public List<Texture> Textures { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        int textureCount = reader.Read<int>();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < textureCount; i++)
            {
                Textures.Add(new Texture
                {
                    Name = reader.ReadStringOffsetOrEmpty()
                });
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Textures.Count);
        writer.WriteOffset(() =>
        {
            foreach (Texture texture in Textures)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, texture.Name);
            }
        });
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        List<string> exception = [];

        for (int i = 0; i < Textures.Count; i++)
        {
            string file = $"{Textures[i].Name}.texture";
            Texture? texture = resolver.Open<Texture>(file);

            if (texture != null)
            {
                Textures[i] = texture;
            }
            else
            {
                exception.Add(file);
            }
        }

        if (exception.Count > 0)
        {
            throw new ResourceResolveException($"Failed to resolve {exception.Count} textures", [.. exception]);
        }
    }

    public override void WriteDependencies(IDirectory dir)
    {
        foreach (Texture texture in Textures)
        {
            texture.Write(dir.CreateFile($"{texture.Name}.texture"));
        }
    }
}
