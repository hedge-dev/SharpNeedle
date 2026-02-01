namespace SharpNeedle.Framework.SurfRide.Draw;

public class ImageCastSurface
{
    public short CropIndex { get; set; }
    public List<CropRef> CropRefs { get; set; } = [];
}

public struct CropRef : IBinarySerializable
{
    public short TextureListIndex;
    public short TextureIndex;
    public short CropIndex;

    public void Read(BinaryObjectReader reader)
    {
        TextureListIndex = reader.ReadInt16();
        TextureIndex = reader.ReadInt16();
        CropIndex = reader.ReadInt16();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt16(TextureListIndex);
        writer.WriteInt16(TextureIndex);
        writer.WriteInt16(CropIndex);
    }
}