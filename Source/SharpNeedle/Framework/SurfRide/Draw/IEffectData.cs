namespace SharpNeedle.Framework.SurfRide.Draw;

public interface IEffectData : IBinarySerializable<ChunkBinaryOptions>
{
    EffectType Type { get; }
}

public enum EffectType : int
{
    None, Blur, Reflect
}