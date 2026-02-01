namespace SharpNeedle.Framework.SonicTeam.DiEvent;

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
    public string Name { get; set; } = string.Empty;
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public int Field5C { get; set; }
    public int[] UnknownArray { get; set; } = [];
    public byte[] Data { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        Version = reader.ReadUInt32();
        Flag = reader.ReadUInt32();
        StartFrame = reader.ReadUInt32();
        EndFrame = reader.ReadUInt32();
        Field10 = reader.ReadInt32();
        int dataSize = reader.ReadInt32();
        SceneEndFrame = reader.ReadInt32();
        Field1C = reader.ReadInt32();
        int unknownCount = reader.ReadInt32();
        Field24 = reader.ReadInt32();
        Field28 = reader.ReadInt32();
        Field2C = reader.ReadInt32();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 32);

        UnknownArray = new int[unknownCount];
        reader.ReadArray<int>(unknownCount, UnknownArray);

        Data = new byte[dataSize];
        reader.ReadArray<byte>(dataSize, Data);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Version);
        writer.WriteUInt32(Flag);
        writer.WriteUInt32(StartFrame);
        writer.WriteUInt32(EndFrame);
        writer.WriteInt32(Field10);
        writer.WriteInt32(Data.Length);
        writer.WriteInt32(SceneEndFrame);
        writer.WriteInt32(Field1C);
        writer.WriteInt32(UnknownArray.Length);
        writer.WriteInt32(Field24);
        writer.WriteInt32(Field28);
        writer.WriteInt32(Field2C);
        writer.WriteString(StringBinaryFormat.FixedLength, Name, 32);

        writer.WriteArray(UnknownArray);
        writer.WriteArray(Data);
    }
}
