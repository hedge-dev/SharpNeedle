namespace SharpNeedle.Frontiers.DVScene;

public class DVSegment : IBinarySerializable
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int StartFrame { get; set; }
    public int EndFrame { get; set; }
    public int Field10 { get; set; }
    public int DataSize { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public int UnknownCount { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public string Name { get; set; }
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public int Field5C { get; set; }
    public int[] UnknownArray { get; set; }
    public byte[] Data { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        StartFrame = reader.Read<int>();
        EndFrame = reader.Read<int>();
        Field10 = reader.Read<int>();
        DataSize = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();
        UnknownCount = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 32);

        UnknownArray = new int[UnknownCount];
        reader.ReadArray<int>(UnknownCount, UnknownArray);

        Data = new byte[DataSize];
        reader.ReadArray<byte>(DataSize, Data);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        writer.Write(Field10);
        writer.Write(DataSize);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(UnknownCount);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.WriteString(StringBinaryFormat.FixedLength, Name, 32);

        writer.WriteArrayFixedLength(UnknownArray, UnknownCount);
        writer.WriteArrayFixedLength(Data, DataSize);
    }
}
