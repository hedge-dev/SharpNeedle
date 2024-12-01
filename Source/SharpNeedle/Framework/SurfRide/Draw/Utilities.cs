namespace SharpNeedle.Framework.SurfRide.Draw;

public static class Utilities
{
    public static IEffectData ReadEffectOffset(BinaryObjectReader reader, EffectType type, ChunkBinaryOptions options)
    {
        switch(type)
        {
            case EffectType.None:
                return null;

            case EffectType.Blur:
                return reader.ReadObjectOffset<BlurEffectData, ChunkBinaryOptions>(options);

            case EffectType.Reflect:
                return reader.ReadObjectOffset<ReflectEffectData, ChunkBinaryOptions>(options);

            default:
                throw new NotImplementedException($"Unimplemented effect type: {type}");
        }
    }
}