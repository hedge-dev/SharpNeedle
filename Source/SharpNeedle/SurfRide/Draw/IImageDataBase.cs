namespace SharpNeedle.SurfRide.Draw;

public interface IImageDataBase : ICastData
{
    public Vector2 Size { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public BlendMode BlendMode { get; set; }
    public UvRotation UvRotation { get; set; }
    public TextureFilterMode TextureFilterMode { get; set; }
    public PivotPosition PivotPosition { get; set; }
    public ImageCastSurface Surface0 { get; set; }
    public ImageCastSurface Surface1 { get; set; }
}
public interface ICastData : IBinarySerializable<ChunkBinaryOptions> { }

public enum BlendMode
{
    Default,
    Add,
    Subtract,
    Multiply = 4,
}

public enum PivotPosition
{
    BottomRight = 0x40000,
    BottomCenter = 0x80000,
    BottomLeft = 0x100000,
    MiddleRight = 0x180000,
    MiddleCenter = 0x200000,
    MiddleLeft = 0x280000,
    TopRight = 0x300000,
    TopCenter = 0x380000,
    TopLeft = 0x400000
}

public enum TextureFilterMode
{
    PointSample,
    Linear
}

public enum UvRotation
{
    FlipHorizontal = 0x10,
    FlipVertical = 0x20,
    D90 = 0x40,
    FlipHorizontalAndVertical = 0x80,
}
