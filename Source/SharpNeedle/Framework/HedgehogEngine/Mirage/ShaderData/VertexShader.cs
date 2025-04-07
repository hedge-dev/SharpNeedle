namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;
[NeedleResource("hh/vertexshader", @"\.vertexshader$")]
public class VertexShader : BaseShader
{
    public override char ByteCodeFileExtensionCharacter => 'v';
    public override string ParameterFileExtension => ".vsparam";
}
