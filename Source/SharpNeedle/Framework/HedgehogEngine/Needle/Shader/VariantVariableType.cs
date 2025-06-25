namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

public enum VariantVariableType : int
{
    // First three need confirmation, only guessed right now,
    // as they are never used in any vanilla shader.
    Boolean = 0,
    Integer = 1,
    Float = 2,

    Texture = 3,
    Sampler = 4,

    ConstantBuffer = 5,
    CBBoolean = 6,
    CBInteger = 7,
    CBFloat = 8,

    // Only used in compute buffers in SXSG, so likely just "compute buffer"
    UnorderedAccessView = 9
}
