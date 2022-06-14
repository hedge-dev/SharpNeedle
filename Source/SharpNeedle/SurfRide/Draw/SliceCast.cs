namespace SharpNeedle.SurfRide.Draw;

public class SliceCast : ICast
{
    public FlagType Flags { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 AnchorPoint { get; set; }
    public uint GradientTopLeft { get; set; }
    public uint GradientBottomLeft { get; set; }
    public uint GradientTopRight { get; set; }
    public uint GradientBottomRight { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public short Field2C { get; set; }
    public short Field2E { get; set; }
    public short Field30 { get; set; }
    public short Field32 { get; set; }
    public short Field36 { get; set; }
    public long Field40 { get; set; }
    public long Field48 { get; set; }
    public List<SliceInfo> Infos { get; set; } = new();
    public List<Slice> Slices { get; set; } = new();

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Flags = reader.Read<FlagType>();
        Width = reader.Read<float>();
        Height = reader.Read<float>();
        AnchorPoint = reader.Read<Vector2>();
        GradientTopLeft = reader.Read<uint>();
        GradientBottomLeft = reader.Read<uint>();
        GradientTopRight = reader.Read<uint>();
        GradientBottomRight = reader.Read<uint>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<short>();
        Field2E = reader.Read<short>();
        Field30 = reader.Read<short>();
        Field32 = reader.Read<short>();
        var infoCount = reader.Read<short>();
        Field36 = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        if (infoCount != 0)
            Infos.AddRange(reader.ReadArrayOffset<SliceInfo>(infoCount));
        else
            reader.ReadOffsetValue();
        Field40 = reader.ReadOffsetValue();
        Field48 = reader.ReadOffsetValue();
        Slices.AddRange(reader.ReadObjectArray<Slice>(Field2C * Field2E));
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
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write<short>(Field2E);
        writer.Write(Field30);
        writer.Write(Field32);
        writer.Write((short)Infos.Count);
        writer.Write(Field36);
        if (options.Version >= 3)
            writer.Align(8);
        if (Infos.Count != 0)
            writer.WriteCollectionOffset(Infos);
        else
            writer.WriteOffsetValue(0);
        writer.WriteOffsetValue(Field40);
        writer.WriteOffsetValue(Field48);
        writer.WriteObjectCollection(Slices);
    }
}

public struct SliceInfo
{
    public ushort Field00;
    public ushort Field02;
    public ushort Field06;
}

public class Slice : IBinarySerializable
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }
    public byte Field0C { get; set; }
    public byte Field0D { get; set; }
    public byte Field0E { get; set; }
    public byte Field0F { get; set; }
    public byte Field10 { get; set; }
    public byte Field11 { get; set; }
    public byte Field12 { get; set; }
    public byte Field13 { get; set; }
    public byte Field14 { get; set; }
    public byte Field15 { get; set; }
    public byte Field16 { get; set; }
    public byte Field17 { get; set; }
    public byte Field18 { get; set; }
    public byte Field19 { get; set; }
    public byte Field1A { get; set; }
    public byte Field1B { get; set; }
    public byte Field1C { get; set; }
    public byte Field1D { get; set; }
    public byte Field1E { get; set; }
    public byte Field1F { get; set; }
    public byte Field20 { get; set; }
    public byte Field21 { get; set; }
    public byte Field22 { get; set; }
    public byte Field23 { get; set; }
    public uint SliceIndex { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<byte>();
        Field0D = reader.Read<byte>();
        Field0E = reader.Read<byte>();
        Field0F = reader.Read<byte>();
        Field10 = reader.Read<byte>();
        Field11 = reader.Read<byte>();
        Field12 = reader.Read<byte>();
        Field13 = reader.Read<byte>();
        Field14 = reader.Read<byte>();
        Field15 = reader.Read<byte>();
        Field16 = reader.Read<byte>();
        Field17 = reader.Read<byte>();
        Field18 = reader.Read<byte>();
        Field19 = reader.Read<byte>();
        Field1A = reader.Read<byte>();
        Field1B = reader.Read<byte>();
        Field1C = reader.Read<byte>();
        Field1D = reader.Read<byte>();
        Field1E = reader.Read<byte>();
        Field1F = reader.Read<byte>();
        Field20 = reader.Read<byte>();
        Field21 = reader.Read<byte>();
        Field22 = reader.Read<byte>();
        Field23 = reader.Read<byte>();
        SliceIndex = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field0D);
        writer.Write(Field0E);
        writer.Write(Field0F);
        writer.Write(Field10);
        writer.Write(Field11);
        writer.Write(Field12);
        writer.Write(Field13);
        writer.Write(Field14);
        writer.Write(Field15);
        writer.Write(Field16);
        writer.Write(Field17);
        writer.Write(Field18);
        writer.Write(Field19);
        writer.Write(Field1A);
        writer.Write(Field1B);
        writer.Write(Field1C);
        writer.Write(Field1D);
        writer.Write(Field1E);
        writer.Write(Field1F);
        writer.Write(Field20);
        writer.Write(Field21);
        writer.Write(Field22);
        writer.Write(Field23);
        writer.Write(SliceIndex);
    }
}