namespace SharpNeedle.BINA;

public interface IChunk : IBinarySerializable<ChunkHeader?>
{
    public uint Signature { get; }

}