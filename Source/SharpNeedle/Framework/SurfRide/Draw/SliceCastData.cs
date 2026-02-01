namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

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
    public IEffectData? Effect { get; set; }
    public ImageCastSurface Surface { get; set; } = new();
    public List<Slice> Slices { get; set; } = [];

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Flags = (CastAttribute)reader.ReadInt32();
        Size = reader.ReadVector2();
        PivotPoint = reader.ReadVector2();
        VertexColorTopLeft = reader.ReadObject<Color<byte>>();
        VertexColorBottomLeft = reader.ReadObject<Color<byte>>();
        VertexColorTopRight = reader.ReadObject<Color<byte>>();
        VertexColorBottomRight = reader.ReadObject<Color<byte>>();
        HorizontalFixedSize = reader.ReadSingle();
        VerticalFixedSize = reader.ReadSingle();
        SliceHorizontalCount = reader.ReadInt16();
        SliceVerticalCount = reader.ReadInt16();
        HorizontalFixedCount = reader.ReadInt16();
        VerticalFixedCount = reader.ReadInt16();
        Surface.CropRefs.Capacity = reader.ReadInt16();
        reader.Skip(2); // Alignment

        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        if (Surface.CropRefs.Capacity != 0)
        {
            Surface.CropRefs.AddRange(reader.ReadObjectArrayOffset<CropRef>(Surface.CropRefs.Capacity));
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
        Slices.AddRange(reader.ReadObjectArray<Slice>(SliceHorizontalCount * SliceVerticalCount));
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
        writer.WriteSingle(HorizontalFixedSize);
        writer.WriteSingle(VerticalFixedSize);
        writer.WriteInt16(SliceHorizontalCount);
        writer.WriteInt16(SliceVerticalCount);
        writer.WriteInt16(HorizontalFixedCount);
        writer.WriteInt16(VerticalFixedCount);
        writer.WriteInt16((short)Surface.CropRefs.Count);
        writer.WriteInt16(0); // Alignment

        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (Surface.CropRefs.Count != 0)
        {
            writer.WriteCollectionOffset(Surface.CropRefs);
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
        Flags = reader.ReadUInt32();
        FixedWidth = reader.ReadSingle();
        FixedHeight = reader.ReadSingle();
        MaterialColor = reader.ReadObject<Color<byte>>();
        IlluminationColor = reader.ReadObject<Color<byte>>();
        VertexColorTopLeft = reader.ReadObject<Color<byte>>();
        VertexColorBottomLeft = reader.ReadObject<Color<byte>>();
        VertexColorTopRight = reader.ReadObject<Color<byte>>();
        VertexColorBottomRight = reader.ReadObject<Color<byte>>();
        CropIndex0 = reader.ReadInt16();
        CropIndex1 = reader.ReadInt16();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Flags);
        writer.WriteSingle(FixedWidth);
        writer.WriteSingle(FixedHeight);
        writer.WriteObject(MaterialColor);
        writer.WriteObject(IlluminationColor);
        writer.WriteObject(VertexColorTopLeft);
        writer.WriteObject(VertexColorBottomLeft);
        writer.WriteObject(VertexColorTopRight);
        writer.WriteObject(VertexColorBottomRight);
        writer.WriteInt16(CropIndex0);
        writer.WriteInt16(CropIndex1);
    }
}