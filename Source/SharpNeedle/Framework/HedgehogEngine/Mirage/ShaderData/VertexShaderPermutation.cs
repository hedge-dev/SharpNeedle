namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;
public class VertexShaderPermutation : BaseShaderPermutation<VertexShaderSubPermutations, VertexShader>
{
    public override string ShaderFileExtension => ".vertexshader";

    public VertexShaderPermutation(VertexShaderSubPermutations subPermutations, string name, string shaderName) : base(subPermutations, name, shaderName) { }

    public VertexShaderPermutation() : base(VertexShaderSubPermutations.None, string.Empty, string.Empty) { }
}