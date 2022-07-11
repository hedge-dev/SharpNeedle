namespace SharpNeedle.SurfRide.Draw;

public class ImageCastSurface
{
    public short CropIndex { get; set; }
    public List<CropRef> CropRefs { get; set; } = new();
}

public struct CropRef
{
    public short TextureListIndex;
    public short TextureIndex;
    public short CropIndex;
}