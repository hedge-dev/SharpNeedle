namespace SharpNeedle.Framework.Ninja;

using SharpNeedle.Framework.Ninja.Csd;

public interface IChunk : IBinarySerializable<ChunkBinaryOptions>
{
    public uint Signature { get; }
}

public struct ChunkBinaryOptions
{
    public ChunkHeader? Header;
    public TextureFormat TextureFormat;
}