namespace SharpNeedle.SurfRide.Draw;

public class SliceCastData : IImageDataBase
{
    public CastAttribute Flags { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public float HorizontalFixedSize { get; set; }
    public float VerticalFixedSize { get; set; }
    public short SliceHorizontalCount { get; set; }
    public short SliceVerticalCount { get; set; }
    public short HorizontalFixedCount { get; set; }
    public short VerticalFixedCount { get; set; }
    public long EventOffset { get; set; }
    public ImageCastSurface Surface0 { get; set; } = new();
    public ImageCastSurface Surface1 { get; set; } = new();
    public List<Slice> Slices { get; set; } = new();

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Flags = reader.Read<CastAttribute>();
        Size = reader.Read<Vector2>();
        PivotPoint = reader.Read<Vector2>();
        VertexColorTopLeft = reader.Read<Color<byte>>();
        VertexColorBottomLeft = reader.Read<Color<byte>>();
        VertexColorTopRight = reader.Read<Color<byte>>();
        VertexColorBottomRight = reader.Read<Color<byte>>();
        HorizontalFixedSize = reader.Read<float>();
        VerticalFixedSize = reader.Read<float>();
        SliceHorizontalCount = reader.Read<short>();
        SliceVerticalCount = reader.Read<short>();
        HorizontalFixedCount = reader.Read<short>();
        VerticalFixedCount = reader.Read<short>();
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

        EventOffset = reader.ReadOffsetValue();
        Slices.AddRange(reader.ReadObjectArray<Slice>(SliceHorizontalCount * SliceVerticalCount));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Flags);
        writer.Write(Size);
        writer.Write(PivotPoint);
        writer.Write(VertexColorTopLeft);
        writer.Write(VertexColorBottomLeft);
        writer.Write(VertexColorTopRight);
        writer.Write(VertexColorBottomRight);
        writer.Write(HorizontalFixedSize);
        writer.Write(VerticalFixedSize);
        writer.Write(SliceHorizontalCount);
        writer.Write(SliceVerticalCount);
        writer.Write(HorizontalFixedCount);
        writer.Write(VerticalFixedCount);
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

        writer.WriteOffsetValue(EventOffset);
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
    public short CropIndex0 { get; set; }
    public short CropIndex1 { get; set; }

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
        CropIndex0 = reader.Read<short>();
        CropIndex1 = reader.Read<short>();
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
        writer.Write(CropIndex0);
        writer.Write(CropIndex1);
    }
}