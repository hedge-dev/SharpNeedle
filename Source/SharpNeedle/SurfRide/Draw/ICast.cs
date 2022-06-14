namespace SharpNeedle.SurfRide.Draw;

public enum FlagType : uint
{
    Transparent = 1,
    InvertColors = 2,
    FlipHorizontally = 0x10,
    FlipVertically = 0x20,
    RotateLeft = 0x40,
    FlipHorizontallyAndVertically = 0x80,
    UseFont = 0x100,
    AntiAliasing = 0x2000,
    AnchorBottomRight = 0x40000,
    AnchorBottom = 0x80000,
    AnchorBottomLeft = 0x100000,
    AnchorRight = 0x180000,
    AnchorCenter = 0x200000,
    AnchorLeft = 0x280000,
    AnchorTopRight = 0x300000,
    AnchorTop = 0x380000,
    AnchorTopLeft = 0x400000
}

public interface ICast : IBinarySerializable<ChunkBinaryOptions> { }
