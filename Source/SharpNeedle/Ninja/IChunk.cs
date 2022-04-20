namespace SharpNeedle.Ninja;

public interface IChunk : IBinarySerializable<ChunkBinaryOptions>
{
    public uint Signature { get; }
}

public struct ChunkBinaryOptions
{
    public ChunkHeader? Header;
}