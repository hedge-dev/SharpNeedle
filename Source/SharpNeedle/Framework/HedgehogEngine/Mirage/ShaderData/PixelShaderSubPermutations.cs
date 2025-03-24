namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

using System;

[Flags]
public enum PixelShaderSubPermutations
{
    None = 1 << 0,
    ConstTexCoord = 1 << 1,
    NoGI = 1 << 2,
    NoGI_ConstTexCoord = 1 << 3,
    NoLight = 1 << 4,
    NoLight_ConstTexCoord = 1 << 5,
    NoLight_NoGI = 1 << 6,
    NoLight_NoGI_ConstTexCoord = 1 << 7,
    All = 0xFF
}
