namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

[NeedleResource("hh/shaderlist", @"\.shader-list$")]
public class ShaderList : SampleChunkResource
{
    public List<PixelShaderPermutation> PixelShaderPermutations { get; } = [];

    public bool HasInputs { get; set; }

    public List<ShaderListParameter> ParameterInputs { get; } = [];

    public List<ShaderListTexture> TextureInputs { get; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        int permutationCount = reader.ReadInt32();
        long permutationOffset = reader.ReadOffsetValue();
        reader.ReadAtOffset(permutationOffset, () =>
        {
            for (int i = 0; i < permutationCount; i++)
            {
                PixelShaderPermutations.Add(reader.ReadObjectOffset<PixelShaderPermutation>());
            }
        });

        HasInputs = permutationOffset > 8;
        if (!HasInputs)
        {
            return;
        }

        int parameterCount = reader.ReadInt32();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < parameterCount; i++)
            {
                ParameterInputs.Add(reader.ReadObjectOffset<ShaderListParameter>());
            }
        });

        int textureCount = reader.ReadInt32();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < textureCount; i++)
            {
                TextureInputs.Add(reader.ReadObjectOffset<ShaderListTexture>());
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

        if (!HasInputs)
        {
            return;
        }

        writer.Write(ParameterInputs.Count);
        writer.WriteOffset(() =>
        {
            foreach (ShaderListParameter parameter in ParameterInputs)
            {
                writer.WriteObjectOffset(parameter, 4);
            }
        }, 4);

        writer.Write(TextureInputs.Count);
        writer.WriteOffset(() =>
        {
            foreach (ShaderListTexture texture in TextureInputs)
            {
                writer.WriteObjectOffset(texture, 4);
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