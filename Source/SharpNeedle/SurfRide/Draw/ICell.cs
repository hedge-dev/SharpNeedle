namespace SharpNeedle.SurfRide.Draw;

public interface ICastCell : IBinarySerializable<ChunkBinaryOptions>
{
    public uint Color { get; }
    public byte Field04 { get; }
    public byte Field05 { get; }
    public byte Field06 { get; }
    public byte Field07 { get; }
    public byte Field08 { get; }
    public byte Field09 { get; }
    public byte Field0A { get; }
    public byte Field0B { get; }
    public Vector3 Position { get; }
}
