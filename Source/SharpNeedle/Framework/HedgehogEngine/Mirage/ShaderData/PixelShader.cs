namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;
[NeedleResource("hh/pixelshader", @"\.pixelshader$")]
public class PixelShader : BaseShader
{
    public override char ByteCodeFileExtensionCharacter => 'p';
    public override string ParameterFileExtension => ".psparam";
}

