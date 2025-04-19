namespace SharpNeedle.SurfRide.Draw;

public class ImageCastSurface : ICloneable
{
    public short CropIndex { get; set; }
    public List<CropRef> CropRefs { get; set; } = new();

    public object Clone()
    {
        ImageCastSurface clone = MemberwiseClone() as ImageCastSurface;
        clone.CropRefs = new(CropRefs);

        return clone;
    }
}

public struct CropRef
{
    public short TextureListIndex;
    public short TextureIndex;
    public short CropIndex;
}