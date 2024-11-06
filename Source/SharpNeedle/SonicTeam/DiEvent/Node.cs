namespace SharpNeedle.SonicTeam.DiEvent;

public class Node : IBinarySerializable<GameType>
{
    public Guid GUID { get; set; }
    public int Type { get; set; }
    public List<Node> Children { get; set; } = new();
    public uint Flags { get; set; }
    public int Priority { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public string Name { get; set; }
    public BaseNodeData Data { get; set; } = new PathData();

    public Node() { }

    public Node(string name)
    {
        Name = name;
        GUID = Guid.NewGuid();
    }

    public Node(string name, NodeType type) : this(name)
    {
        Type = (int)type;
    }

    public Node(string name, NodeType type, BaseNodeData data) : this(name, type)
    {
        Data = data;
    }

    public Node(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        GUID = reader.Read<Guid>();
        Type = reader.Read<int>();
        int dataSize = reader.Read<int>();

        int childCount = reader.Read<int>();
        Flags = reader.Read<uint>();

        Priority = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();

        Name = reader.ReadDiString(64);

        switch ((NodeType)Type)
        {
            case NodeType.Path:
                Data = new PathData(reader, game);
                break;

            case NodeType.Camera:
                Data = new CameraData(reader, game);
                break;

            case NodeType.CameraMotion:
                Data = new CameraMotionData(reader, game);
                break;

            case NodeType.ModelCustom:
            case NodeType.Character:
                Data = new ModelData(reader, game);
                break;

            case NodeType.CharacterMotion:
            case NodeType.ModelMotion:
                Data = new MotionModelData(reader, game);
                break;

            case NodeType.Attachment:
                Data = new AttachmentData(reader, game);
                break;

            case NodeType.Parameter:
                Data = new ParameterData(reader, game, dataSize);
                break;

            default:
                reader.Skip(dataSize * 4);
                break;
        }

        Children.AddRange(reader.ReadObjectArray<Node, GameType>(game, childCount));
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(GUID);
        writer.Write(Type);

        long dataSizePos = writer.Position;
        writer.WriteNulls(4);

        writer.Write(Children.Count);
        writer.Write(Flags);

        writer.Write(Priority);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);

        writer.WriteDiString(Name, 64);

        long dataStart = writer.Position;
        Data.Write(writer, game);
        long dataEnd = writer.Position;

        writer.Seek(dataSizePos, System.IO.SeekOrigin.Begin);
        writer.Write((int)(dataEnd - dataStart) / 4);
        writer.Seek(dataEnd, System.IO.SeekOrigin.Begin);

        writer.WriteObjectCollection(game, Children);
    }
}