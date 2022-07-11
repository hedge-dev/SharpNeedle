namespace SharpNeedle.SurfRide.Draw;

public class SliceCast : IImageDataBase
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public short SliceHorizontalCount { get; set; }
    public short SliceVerticalCount { get; set; }
    public long Field40 { get; set; }
    public BlendMode BlendMode { get; set; }
    public UvRotation UvRotation { get; set; }
    public TextureFilterMode TextureFilterMode { get; set; }
    public PivotPosition PivotPosition { get; set; }
    public ImageCastSurface Surface0 { get; set; } = new();
    public ImageCastSurface Surface1 { get; set; } = new();
    public List<Slice> Slices { get; set; } = new();

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        var flags = reader.Read<uint>();
        BlendMode = (BlendMode)(flags & 0xF);
        PivotPosition = (PivotPosition)(flags & 0xFF0000);
        TextureFilterMode = (flags & 0x2000) == 0x2000 ? TextureFilterMode.PointSample : TextureFilterMode.Linear;
        UvRotation = (UvRotation)(flags & 0xF0);

        Width = reader.Read<float>();
        Height = reader.Read<float>();
        PivotPoint = reader.Read<Vector2>();
        VertexColorTopLeft = reader.Read<Color<byte>>();
        VertexColorBottomLeft = reader.Read<Color<byte>>();
        VertexColorTopRight = reader.Read<Color<byte>>();
        VertexColorBottomRight = reader.Read<Color<byte>>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        SliceHorizontalCount = reader.Read<short>();
        SliceVerticalCount = reader.Read<short>();
        Surface0.CropIndex = reader.Read<short>();
        Surface1.CropIndex = reader.Read<short>();
        Surface0.CropRefs.Capacity = reader.Read<short>();
        Surface1.CropRefs.Capacity = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        
        if (Surface0.CropRefs.Capacity != 0)
            Surface0.CropRefs.AddRange(reader.ReadArrayOffset<CropRef>(Surface0.CropRefs.Capacity));
        else
            reader.ReadOffsetValue();

        if (Surface1.CropRefs.Capacity != 0)
            Surface1.CropRefs.AddRange(reader.ReadArrayOffset<CropRef>(Surface1.CropRefs.Capacity));
        else
            reader.ReadOffsetValue();

        Field40 = reader.ReadOffsetValue();
        Slices.AddRange(reader.ReadObjectArray<Slice>(SliceHorizontalCount * SliceVerticalCount));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        int flags = (int)BlendMode + (int)UvRotation + (int)PivotPosition;
        if (TextureFilterMode == TextureFilterMode.PointSample)
            flags += 0x2000;

        writer.Write(flags);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(PivotPoint);
        writer.Write(VertexColorTopLeft);
        writer.Write(VertexColorBottomLeft);
        writer.Write(VertexColorTopRight);
        writer.Write(VertexColorBottomRight);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(SliceHorizontalCount);
        writer.Write(SliceVerticalCount);
        writer.Write(Surface0.CropIndex);
        writer.Write(Surface1.CropIndex);
        writer.Write((short)Surface0.CropRefs.Count);
        writer.Write((short)Surface1.CropRefs.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Surface0.CropRefs.Count != 0)
            writer.WriteCollectionOffset(Surface0.CropRefs);
        else
            writer.WriteOffsetValue(0);

        if (Surface1.CropRefs.Count != 0)
            writer.WriteCollectionOffset(Surface1.CropRefs);
        else
            writer.WriteOffsetValue(0);

        writer.WriteOffsetValue(Field40);
        writer.WriteObjectCollection(Slices);
    }
}

public class Slice : IBinarySerializable
{
    public uint Flags { get; set; }
    public float FixedWidth { get; set; }
    public float FixedHeight { get; set; }
    public Color<byte> MaterialColor { get; set; }
    public Color<byte> IlluminationColor { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public uint SliceIndex { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Flags = reader.Read<uint>();
        FixedWidth = reader.Read<float>();
        FixedHeight = reader.Read<float>();
        MaterialColor = reader.Read<Color<byte>>();
        IlluminationColor = reader.Read<Color<byte>>();
        VertexColorTopLeft = reader.Read<Color<byte>>();
        VertexColorBottomLeft = reader.Read<Color<byte>>();
        VertexColorTopRight = reader.Read<Color<byte>>();
        VertexColorBottomRight = reader.Read<Color<byte>>();
        SliceIndex = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Flags);
        writer.Write(FixedWidth);
        writer.Write(FixedHeight);
        writer.Write(MaterialColor);
        writer.Write(IlluminationColor);
        writer.Write(VertexColorTopLeft);
        writer.Write(VertexColorBottomLeft);
        writer.Write(VertexColorTopRight);
        writer.Write(VertexColorBottomRight);
        writer.Write(SliceIndex);
    }
}