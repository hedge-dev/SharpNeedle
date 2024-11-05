namespace SharpNeedle.SonicTeam.DiEvent;

public class Page : IBinarySerializable
{
    public uint Version { get; set; }
    public uint Flag { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public int Field10 { get; set; }
    public int SceneEndFrame { get; set; }
    public int Field1C { get; set; }
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
        Version = reader.Read<uint>();
        Flag = reader.Read<uint>();
        StartFrame = reader.Read<uint>();
        EndFrame = reader.Read<uint>();
        Field10 = reader.Read<int>();
        int dataSize = reader.Read<int>();
        SceneEndFrame = reader.Read<int>();
        Field1C = reader.Read<int>();
        int unknownCount = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 32);

        UnknownArray = new int[unknownCount];
        reader.ReadArray<int>(unknownCount, UnknownArray);

        Data = new byte[dataSize];
        reader.ReadArray<byte>(dataSize, Data);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Version);
        writer.Write(Flag);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        writer.Write(Field10);
        writer.Write(Data.Length);
        writer.Write(SceneEndFrame);
        writer.Write(Field1C);
        writer.Write(UnknownArray.Length);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.WriteString(StringBinaryFormat.FixedLength, Name, 32);

        writer.WriteArray(UnknownArray);
        writer.WriteArray(Data);
    }
}
