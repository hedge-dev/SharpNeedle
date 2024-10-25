namespace SharpNeedle.SonicTeam.DivScene;

public class DivNode : IBinarySerializable
{
    public Guid GUID { get; set; }
    public int Type { get; set; }
    public List<DivNode> Children { get; set; } = new();
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public string Name { get; set; }
    public DivNodeData Data { get; set; } = new DivTransformData();

    public DivNode() { }

    public DivNode(string name)
    {
        Name = name;
        GUID = Guid.NewGuid();
    }

    public DivNode(string name, NodeType type) : this(name)
    {
        Type = (int)type;
    }

    public DivNode(string name, NodeType type, DivNodeData data) : this(name, type)
    {
        Data = data;
    }

    public void Read(BinaryObjectReader reader)
    {
        GUID = reader.Read<Guid>();
        Type = reader.Read<int>();
        int dataSize = reader.Read<int>();

        int childCount = reader.Read<int>();
        Field1C = reader.Read<int>();

        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();

        Name = reader.ReadDivString(64);

        switch ((NodeType)Type)
        {
            case NodeType.Transform:
                Data = new DivTransformData(reader);
                break;

            case NodeType.Camera:
                Data = new DivCameraData(reader);
                break;

            case NodeType.CameraMotion:
                Data = new DivCameraMotionData(reader);
                break;

            case NodeType.Model:
            case NodeType.Model2:
                Data = new DivModelData(reader);
                break;

            case NodeType.Motion:
            case NodeType.Motion2:
                Data = new DivMotionData(reader);
                break;

            case NodeType.NodeAttachment:
                Data = new DivNodeAttachData(reader);
                break;

            case NodeType.Parameter:
                Data = new DivParameterData(reader, dataSize);
                break;

            default:
                reader.Skip(dataSize * 4);
                break;
        }

        Children.AddRange(reader.ReadObjectArray<DivNode>(childCount));
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

        writer.WriteDivString(Name, 64);

        long dataStart = writer.Position;
        Data.Write(writer);
        long dataEnd = writer.Position;

        writer.Seek(dataSizePos, System.IO.SeekOrigin.Begin);
        writer.Write((int)(dataEnd - dataStart) / 4);
        writer.Seek(dataEnd, System.IO.SeekOrigin.Begin);

        writer.WriteObjectCollection(Children);
    }
}