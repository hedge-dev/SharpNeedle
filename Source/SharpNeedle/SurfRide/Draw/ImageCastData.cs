namespace SharpNeedle.SurfRide.Draw;

public class ImageCastData : IImageDataBase
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public long Field38 { get; set; }
    public long Field3C { get; set; }
    public BlendMode BlendMode { get; set; }
    public UvRotation UvRotation { get; set; }
    public TextureFilterMode TextureFilterMode { get; set; }
    public PivotPosition PivotPosition { get; set; }
    public ImageCastSurface Surface0 { get; set; } = new();
    public ImageCastSurface Surface1 { get; set; } = new();
    public FontData FontData { get; set; }

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
        Surface0.CropIndex = reader.Read<short>();
        Surface1.CropIndex = reader.Read<short>();
        Surface0.CropRefs.Capacity = reader.Read<short>();
        Surface1.CropRefs.Capacity = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        
        if (Surface0.CropRefs.Capacity > 0)
            Surface0.CropRefs.AddRange(reader.ReadArrayOffset<CropRef>(Surface0.CropRefs.Capacity));
        else
            reader.ReadOffsetValue();

        if (Surface1.CropRefs.Capacity > 0)
            Surface1.CropRefs.AddRange(reader.ReadArrayOffset<CropRef>(Surface1.CropRefs.Capacity));
        else
            reader.ReadOffsetValue();
        
        if ((flags & 0xF00) == 0x100)
            FontData = reader.ReadObjectOffset<FontData, ChunkBinaryOptions>(options);
        else
            reader.ReadOffsetValue();
        
        Field38 = reader.ReadOffsetValue();
        Field3C = reader.ReadOffsetValue();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        int flags = (int)BlendMode + (int)UvRotation + (int)PivotPosition;
        if (FontData != null)
            flags += 0x100;

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
        
        if (FontData != null)
            writer.WriteObjectOffset(FontData, options);
        else
            writer.WriteOffsetValue(0);
        
        writer.WriteOffsetValue(Field38);
        writer.WriteOffsetValue(Field3C);
    }
}

public class FontData : IBinarySerializable<ChunkBinaryOptions>
{
    public uint Field00 { get; set; }
    public uint FontListIndex { get; set; }
    public Vector2 Scale { get; set; }
    public uint Field14 { get; set; }
    public uint Field18 { get; set; }
    public short SpaceCorrection { get; set; }
    public ushort Field1E { get; set; }
    public string Characters { get; set; }
    public Font Font { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Field00 = reader.Read<uint>();
        FontListIndex = reader.Read<uint>();
        if (options.Version >= 3)
            reader.Align(8);
        
        Characters = reader.ReadStringOffset();
        Scale = reader.Read<Vector2>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<uint>();
        SpaceCorrection = reader.Read<short>();
        Field1E = reader.Read<ushort>();
        if (options.Version >= 3)
            reader.Align(8);
        
        Font = reader.ReadObjectOffset<Font, ChunkBinaryOptions>(options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Field00);
        writer.Write(FontListIndex);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Characters);
        writer.Write(Scale);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(SpaceCorrection);
        writer.Write(Field1E);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectOffset(Font, options);
    }
}