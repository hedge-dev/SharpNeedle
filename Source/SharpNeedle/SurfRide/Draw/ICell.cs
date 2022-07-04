namespace SharpNeedle.SurfRide.Draw;

public interface ICell : IBinarySerializable<ChunkBinaryOptions>
{
    public Color<byte> Color { get; set; }
    public byte Field04 { get; set; }
    public byte Field05 { get; set; }
    public byte Field06 { get; set; }
    public byte Field07 { get; set; }
    public byte Field08 { get; set; }
    public byte Field09 { get; set; }
    public byte Field0A { get; set; }
    public byte Field0B { get; set; }
    public Vector3 Position { get; set; }
}
