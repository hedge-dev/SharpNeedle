namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;
using System;

[Flags]
public enum VertexShaderSubPermutations
{
    None = 1 << 0,
    ConstTexCoord = 1 << 1,
    All = 0x3
}
