namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

[Flags]
public enum VertexShaderSubPermutations
{
    None = 1 << 0,
    ConstTexCoord = 1 << 1,
    All = 0x3
}

[Flags]
public enum VertexShaderSubPermutationFlags
{
    None = 0,
    ConstTexCoord = 1 << 0,
}

public class VertexShaderPermutation : BaseShaderPermutation<VertexShaderSubPermutations, VertexShaderSubPermutationFlags, VertexShader>
{
    public override string ShaderFileExtension => ".vertexshader";

    public override VertexShaderSubPermutationFlags SubPermutationFlagMask => VertexShaderSubPermutationFlags.ConstTexCoord;

    public VertexShaderPermutation(VertexShaderSubPermutations subPermutations, string name, string shaderName) : base(subPermutations, name, shaderName) { }

    public VertexShaderPermutation() : base(default, string.Empty, string.Empty) { }
}