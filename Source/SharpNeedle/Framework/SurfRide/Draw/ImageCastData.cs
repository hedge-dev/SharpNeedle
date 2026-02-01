namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public class ImageCastData : IImageDataBase
{
    public CastAttribute Flags { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 PivotPoint { get; set; }
    public Color<byte> VertexColorTopLeft { get; set; }
    public Color<byte> VertexColorBottomLeft { get; set; }
    public Color<byte> VertexColorTopRight { get; set; }
    public Color<byte> VertexColorBottomRight { get; set; }
    public IEffectData? Effect { get; set; }
    public ImageCastSurface Surface { get; set; } = new();
    public ImageCastSurface Surface1 { get; set; } = new();
    public FontData? FontData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Flags = (CastAttribute)reader.ReadInt32();
        Size = reader.ReadVector2();
        PivotPoint = reader.ReadVector2();
        VertexColorTopLeft = reader.ReadObject<Color<byte>>();
        VertexColorBottomLeft = reader.ReadObject<Color<byte>>();
        VertexColorTopRight = reader.ReadObject<Color<byte>>();
        VertexColorBottomRight = reader.ReadObject<Color<byte>>();
        Surface.CropIndex = reader.ReadInt16();
        Surface1.CropIndex = reader.ReadInt16();
        Surface.CropRefs.Capacity = reader.ReadInt16();
        Surface1.CropRefs.Capacity = reader.ReadInt16();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        if (Surface.CropRefs.Capacity > 0)
        {
            Surface.CropRefs.AddRange(reader.ReadObjectArrayOffset<CropRef>(Surface.CropRefs.Capacity));
        }
        else
        {
            reader.ReadOffsetValue();
        }

        if (Surface1.CropRefs.Capacity > 0)
        {
            Surface1.CropRefs.AddRange(reader.ReadObjectArrayOffset<CropRef>(Surface1.CropRefs.Capacity));
        }
        else
        {
            reader.ReadOffsetValue();
        }

        if ((Flags & CastAttribute.UseFont) == CastAttribute.UseFont)
        {
            FontData = reader.ReadObjectOffset<FontData, ChunkBinaryOptions>(options);
        }
        else
        {
            reader.ReadOffsetValue();
        }

        EffectType type = (EffectType)reader.ReadInt32();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Effect = Utilities.ReadEffectOffset(reader, type, options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteInt32((int)Flags);
        writer.WriteVector2(Size);
        writer.WriteVector2(PivotPoint);
        writer.WriteObject(VertexColorTopLeft);
        writer.WriteObject(VertexColorBottomLeft);
        writer.WriteObject(VertexColorTopRight);
        writer.WriteObject(VertexColorBottomRight);
        writer.WriteInt16(Surface.CropIndex);
        writer.WriteInt16(Surface1.CropIndex);
        writer.WriteInt16((short)Surface.CropRefs.Count);
        writer.WriteInt16((short)Surface1.CropRefs.Count);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (Surface.CropRefs.Count != 0)
        {
            writer.WriteObjectCollectionOffset(Surface.CropRefs);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (Surface1.CropRefs.Count != 0)
        {
            writer.WriteObjectCollectionOffset(Surface1.CropRefs);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (FontData != null)
        {
            writer.WriteObjectOffset(FontData, options);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.WriteInt32((int)(Effect?.Type ?? EffectType.None));
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteObjectOffset(Effect, options, 16);
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
    public string? Characters { get; set; }
    public Font? Font { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Field00 = reader.ReadUInt32();
        FontListIndex = reader.ReadUInt32();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Characters = reader.ReadStringOffset();
        Scale = reader.ReadVector2();
        Field14 = reader.ReadUInt32();
        Field18 = reader.ReadUInt32();
        SpaceCorrection = reader.ReadInt16();
        Field1E = reader.ReadUInt16();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Font = reader.ReadObjectOffset<Font, ChunkBinaryOptions>(options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteUInt32(Field00);
        writer.WriteUInt32(FontListIndex);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Characters);
        writer.WriteVector2(Scale);
        writer.WriteUInt32(Field14);
        writer.WriteUInt32(Field18);
        writer.WriteInt16(SpaceCorrection);
        writer.WriteUInt16(Field1E);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (Font == null)
        {
            throw new InvalidOperationException("Font is null!");
        }

        writer.WriteObjectOffset(Font, options);
    }
}