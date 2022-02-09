namespace SharpNeedle.BINA;

public interface IChunk : IBinarySerializable<ChunkParseOptions>
{
    public uint Signature { get; }
}

public struct ChunkParseOptions
{
    public ChunkHeader? Header;
    public BinaryResource Owner;
}