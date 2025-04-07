namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;
[NeedleResource("hh/shaderlist", @"\.shader-list$")]
public class ShaderList : SampleChunkResource
{
    public List<PixelShaderPermutation> PixelShaderPermutations { get; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        int count = reader.ReadInt32();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
            {
                PixelShaderPermutations.Add(reader.ReadObjectOffset<PixelShaderPermutation>());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(PixelShaderPermutations.Count);
        writer.WriteOffset(() =>
        {
            foreach (PixelShaderPermutation pixelShaderPermutation in PixelShaderPermutations)
            {
                writer.WriteObjectOffset(pixelShaderPermutation, 4);
            }
        }, 4);
    }

    public override void ResolveDependencies(IResourceResolver dir)
    {
        foreach (PixelShaderPermutation permutation in PixelShaderPermutations)
        {
            permutation.ResolveDependencies(dir);
        }
    }
}