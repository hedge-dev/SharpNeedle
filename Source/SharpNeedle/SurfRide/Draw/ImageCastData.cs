namespace SharpNeedle.SurfRide.Draw;

public class ImageCastData : IImageDataBase
{
    public CastAttribute Flags { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public IEffectData Effect { get; set; }
    public ImageCastSurface Surface { get; set; } = new();
    public ImageCastSurface Surface1 { get; set; } = new();
    public TextData TextData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Flags = reader.Read<CastAttribute>();
        Size = reader.Read<Vector2>();
        PivotPoint = reader.Read<Vector2>();
        VertexColorTopLeft = reader.Read<Color<byte>>();
        VertexColorBottomLeft = reader.Read<Color<byte>>();
        VertexColorTopRight = reader.Read<Color<byte>>();
        VertexColorBottomRight = reader.Read<Color<byte>>();
        Surface.CropIndex = reader.Read<short>();
        Surface1.CropIndex = reader.Read<short>();
        Surface.CropRefs.Capacity = reader.Read<short>();
        Surface1.CropRefs.Capacity = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        
        if (Surface.CropRefs.Capacity > 0)
            Surface.CropRefs.AddRange(reader.ReadArrayOffset<CropRef>(Surface.CropRefs.Capacity));
        else
            reader.ReadOffsetValue();

        if (Surface1.CropRefs.Capacity > 0)
            Surface1.CropRefs.AddRange(reader.ReadArrayOffset<CropRef>(Surface1.CropRefs.Capacity));
        else
            reader.ReadOffsetValue();
        
        if ((Flags & CastAttribute.UseFont) == CastAttribute.UseFont)
            TextData = reader.ReadObjectOffset<TextData, ChunkBinaryOptions>(options);
        else
            reader.ReadOffsetValue();

        var type = reader.Read<EffectType>();
        if (options.Version >= 3)
            reader.Align(8);

        Effect = Utilities.ReadEffectOffset(reader, type, options);
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
        writer.Write(Surface.CropIndex);
        writer.Write(Surface1.CropIndex);
        writer.Write((short)Surface.CropRefs.Count);
        writer.Write((short)Surface1.CropRefs.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Surface.CropRefs.Count != 0)
            writer.WriteCollectionOffset(Surface.CropRefs);
        else
            writer.WriteOffsetValue(0);
        
        if (Surface1.CropRefs.Count != 0)
            writer.WriteCollectionOffset(Surface1.CropRefs);
        else
            writer.WriteOffsetValue(0);
        
        if (TextData != null)
            writer.WriteObjectOffset(TextData, options);
        else
            writer.WriteOffsetValue(0);

        writer.Write(Effect?.Type ?? EffectType.None);
        if (options.Version >= 3)
            writer.Align(8);

        writer.WriteObjectOffset(Effect, options, 16);
    }
}

public class TextData : IBinarySerializable<ChunkBinaryOptions>
{
    public uint Field00 { get; set; }
    public int FontIndex { get; set; }
    public Vector2 Scale { get; set; }
    public short Field14 { get; set; }
    public short Field16 { get; set; }
    public uint Field18 { get; set; }
    public short SpaceCorrection { get; set; }
    public ushort Field1E { get; set; }
    public string Text { get; set; }
    public Font Font { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Field00 = reader.Read<uint>();
        FontIndex = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);

        Text = reader.ReadStringOffset();
        Scale = reader.Read<Vector2>();
        Field14 = reader.Read<short>();
        Field16 = reader.Read<short>();
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
        writer.Write(FontIndex);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Text);
        writer.Write(Scale);
        writer.Write(Field14);
        writer.Write(Field16);
        writer.Write(Field18);
        writer.Write(SpaceCorrection);
        writer.Write(Field1E);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectOffset(Font, options);
    }
}