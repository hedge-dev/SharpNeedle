namespace SharpNeedle.SurfRide.Draw;

public class ImageCast : ICast
{
    public uint Flags { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 AnchorPoint { get; set; }
    public uint GradientTopLeft { get; set; }
    public uint GradientBottomLeft { get; set; }
    public uint GradientTopRight { get; set; }
    public uint GradientBottomRight { get; set; }
    public short Field24 { get; set; }
    public short Field26 { get; set; }
    public long Field38 { get; set; }
    public long Field40 { get; set; }
    public List<Pattern> Patterns { get; set; } = new();
    public List<UnknownInfo> Unknowns { get; set; } = new();
    public FontData FontData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Flags = reader.Read<uint>();
        Width = reader.Read<float>();
        Height = reader.Read<float>();
        AnchorPoint = reader.Read<Vector2>();
        GradientTopLeft = reader.Read<uint>();
        GradientBottomLeft = reader.Read<uint>();
        GradientTopRight = reader.Read<uint>();
        GradientBottomRight = reader.Read<uint>();
        Field24 = reader.Read<short>();
        Field26 = reader.Read<short>();
        var patternInfoCount = reader.Read<ushort>();
        var unknownInfoCount = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        
        if (patternInfoCount > 0)
            Patterns.AddRange(reader.ReadArrayOffset<Pattern>(patternInfoCount));
        else
            reader.ReadOffsetValue();
        
        if (unknownInfoCount > 0)
            Unknowns.AddRange(reader.ReadArrayOffset<UnknownInfo>(unknownInfoCount));
        else
            reader.ReadOffsetValue();
        
        if ((Flags & 0xF00) == 0x100)
            FontData = reader.ReadObjectOffset<FontData, ChunkBinaryOptions>(options);
        else
            reader.ReadOffsetValue();
        
        Field38 = reader.ReadOffsetValue();
        Field40 = reader.ReadOffsetValue();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Flags);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(AnchorPoint);
        writer.Write(GradientTopLeft);
        writer.Write(GradientBottomLeft);
        writer.Write(GradientTopRight);
        writer.Write(GradientBottomRight);
        writer.Write(Field24);
        writer.Write(Field26);
        writer.Write((ushort)Patterns.Count);
        writer.Write((ushort)Unknowns.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Patterns.Count != 0)
            writer.WriteCollectionOffset(Patterns);
        else
            writer.WriteOffsetValue(0);
        
        if (Unknowns.Count != 0)
            writer.WriteCollectionOffset(Unknowns);
        else
            writer.WriteOffsetValue(0);
        
        if (FontData != null)
            writer.WriteObjectOffset(FontData, options);
        else
            writer.WriteOffsetValue(0);
        
        writer.WriteOffsetValue(Field38);
        writer.WriteOffsetValue(Field40);
    }
}

public struct Pattern
{
    public ushort TextureListIndex;
    public ushort TextureIndex;
    public ushort SpriteIndex;
}

public struct UnknownInfo
{
    public short Field00;
    public short Field02;
    public short Field04;
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