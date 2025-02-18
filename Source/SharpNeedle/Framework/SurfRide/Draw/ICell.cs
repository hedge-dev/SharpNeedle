namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public interface ICell : IBinarySerializable<ChunkBinaryOptions>
{
    public Color<byte> MaterialColor { get; set; }
    public Color<byte> IlluminationColor { get; set; }
    public byte Field08 { get; set; }
    public byte Field09 { get; set; }
    public byte Field0A { get; set; }
    public byte Field0B { get; set; }
}
