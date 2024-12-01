﻿namespace SharpNeedle.Framework.SurfRide.Draw;

public class ImageCastSurface
{
    public short CropIndex { get; set; }
    public List<CropRef> CropRefs { get; set; } = [];
}

public struct CropRef
{
    public short TextureListIndex;
    public short TextureIndex;
    public short CropIndex;
}