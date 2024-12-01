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
            for(int i = 0; i < textureCount; i++)
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
            foreach(Texture texture in Textures)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, texture.Name);
            }
        });
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        for(int i = 0; i < Textures.Count; i++)
        {
            Textures[i] = resolver.Open<Texture>($"{Textures[i].Name}.texture");
        }
    }

    public override void WriteDependencies(IDirectory dir)
    {
        foreach(Texture texture in Textures)
        {
            texture.Write(dir.CreateFile($"{texture.Name}.texture"));
        }
    }
}
