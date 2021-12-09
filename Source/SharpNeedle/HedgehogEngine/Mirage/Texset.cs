namespace SharpNeedle.HedgehogEngine.Mirage;

// ok so how does this work, cause SamepleChunkResource.Read is gonna try reading the header lol
// wait nvm
// we'll just pass the read function lol????????????
// i forgor i made that
// make a texture file too

[BinaryResource("hh/texset", @"\.texset$")]
public class Texset : SampleChunkResource
{
    public List<Texture> Textures { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        int textureCount = reader.Read<int>();
        reader.ReadOffset(() => 
        {
            for (int i = 0; i < textureCount; i++)
            {
                Textures.Add(new Texture 
                {
                    Name = reader.ReadStringOffset()
                });
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Textures.Count);
        writer.WriteOffset(() =>
        {
            foreach (var texture in Textures)
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, texture.Name);
        });
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        for (int i = 0; i < Textures.Count; i++)
            Textures[i] = resolver.Open<Texture>($"{Textures[i].Name}.texture");
    }

    public override void WriteDependencies(IDirectory dir)
    {
        foreach (var texture in Textures)
            texture.Write(dir.CreateFile($"{texture.Name}.texture"));
    }
}
