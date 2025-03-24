namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

using SharpNeedle.IO;
using SharpNeedle.Resource;

public class PixelShaderPermutation : BaseShaderPermutation<PixelShaderSubPermutations, PixelShader>
{
    public List<VertexShaderPermutation> VertexShaderPermutations { get; } = [];

    public override string ShaderFileExtension => ".pixelshader";

    public PixelShaderPermutation(PixelShaderSubPermutations subPermutations, string permutationName, string shaderName) : base(subPermutations, permutationName, shaderName) { }

    public PixelShaderPermutation() : base(PixelShaderSubPermutations.None, string.Empty, string.Empty) { }

    public override void Read(BinaryObjectReader reader)
    {
        base.Read(reader);

        int count = reader.ReadInt32();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
            {
                VertexShaderPermutations.Add(reader.ReadObjectOffset<VertexShaderPermutation>());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        base.Write(writer);

        writer.Write(VertexShaderPermutations.Count);
        writer.WriteOffset(() =>
        {
            foreach (VertexShaderPermutation vertexShaderPermutation in VertexShaderPermutations)
            {
                writer.WriteObjectOffset(vertexShaderPermutation, 4);
            }
        }, 4);
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        ResourceResolveException? innerException = null;

        try
        {
            base.ResolveDependencies(resolver);
        }
        catch (ResourceResolveException resourceException)
        {
            innerException = resourceException;
        }

        List<string> unresolved = [];

        foreach (VertexShaderPermutation vertexShaderPermutation in VertexShaderPermutations)
        {
            try
            {
                vertexShaderPermutation.ResolveDependencies(resolver);
            }
            catch (ResourceResolveException resourceException)
            {
                unresolved.AddRange(resourceException.GetRecursiveResources());
            }
        }

        if (unresolved.Count > 0)
        {
            throw new ResourceResolveException("Failed to resolve one or more shader resources", unresolved.ToArray(), innerException);
        }
        else if (innerException != null)
        {
            throw innerException;
        }
    }

    public override void WriteDependencies(IDirectory directory)
    {
        base.WriteDependencies(directory);

        foreach (VertexShaderPermutation vertexShaderPermutation in VertexShaderPermutations)
        {
            vertexShaderPermutation.WriteDependencies(directory);
        }
    }
}