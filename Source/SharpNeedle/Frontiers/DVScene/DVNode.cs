namespace SharpNeedle.Frontiers.DVScene;

public class DVNode : IBinarySerializable
{
    public DVGuid GUID { get; set; }
    public int Type { get; set; }
    public List<DVNode> Children { get; set; } = new();
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public string Name { get; set; }
    public DVNodeData Data { get; set; } = new DVTransformData();

    public void Read(BinaryObjectReader reader)
    {
        GUID = reader.Read<DVGuid>();
        Type = reader.Read<int>();
        int dataSize = reader.Read<int>();

        int childCount = reader.Read<int>();
        Field1C = reader.Read<int>();

        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();

        Name = reader.ReadDVString(64);

        switch (Type)
        {
            case 1:
                Data = new DVTransformData(reader);
                break;

            case 3:
                Data = new DVCameraData(reader);
                break;

            case 4:
                Data = new DVCameraMotionData(reader);
                break;

            case 5:
            case 8:
                Data = new DVModelData(reader);
                break;

            case 6:
            case 10:
                Data = new DVMotionData(reader);
                break;

            case 11:
                Data = new DVNodeAttachData(reader);
                break;

            case 12:
                Data = new DVParameterData(reader, dataSize);
                break;

            default:
                reader.Skip(dataSize * 4);
                break;
        }

        Children.AddRange(reader.ReadObjectArray<DVNode>(childCount));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(GUID);
        writer.Write(Type);

        long dataSizePos = writer.Position;
        writer.WriteNulls(4);

        writer.Write(Children.Count);
        writer.Write(Field1C);

        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);

        writer.WriteDVString(Name, 64);

        long dataStart = writer.Position;
        Data.Write(writer);
        long dataEnd = writer.Position;

        writer.Seek(dataSizePos, System.IO.SeekOrigin.Begin);
        writer.Write((int)(dataEnd - dataStart) / 4);
        writer.Seek(dataEnd, System.IO.SeekOrigin.Begin);

        writer.WriteObjectCollection(Children);
    }
}