namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public interface IImageDataBase : ICastData
{
    public CastAttribute Flags { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public ImageCastSurface Surface { get; set; }
    public IEffectData? Effect { get; set; }
}
public interface ICastData : IBinarySerializable<ChunkBinaryOptions> { }