namespace SharpNeedle.SurfRide;

public interface IChunk : IBinarySerializable<ChunkBinaryOptions>
{
    public uint Signature { get; }
}

public struct ChunkBinaryOptions
{
    public ChunkHeader? Header;
    public uint Version;
}