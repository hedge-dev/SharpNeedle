namespace SharpNeedle.SonicTeam.DivScene;

public class DivNode : IDivDataBlock
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
    public IDivDataBlock Data { get; set; } = new DivDPath();

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

    public DivNode(string name, NodeType type, IDivDataBlock data) : this(name, type)
    {
        Data = data;
    }

    public DivNode(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
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
            case NodeType.Path:
                Data = new DivDPath(reader, game);
                break;

            case NodeType.Camera:
                Data = new DivDCamera(reader, game);
                break;

            case NodeType.CameraMotion:
                Data = new DivDCameraMotion(reader, game);
                break;

            case NodeType.ModelCustom:
            case NodeType.Character:
                Data = new DivDModel(reader, game);
                break;

            case NodeType.CharacterMotion:
            case NodeType.ModelMotion:
                Data = new DivDMotionModel(reader, game);
                break;

            case NodeType.Attachment:
                Data = new DivDAttachment(reader, game);
                break;

            case NodeType.Parameter:
                Data = new DivDParameter(reader, game, dataSize);
                break;

            default:
                reader.Skip(dataSize * 4);
                break;
        }

        for(int i=0; i<childCount; i++)
            Children.Add(new DivNode(reader, game));
    }

    public void Write(BinaryObjectWriter writer, GameType game)
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
        Data.Write(writer, game);
        long dataEnd = writer.Position;

        writer.Seek(dataSizePos, System.IO.SeekOrigin.Begin);
        writer.Write((int)(dataEnd - dataStart) / 4);
        writer.Seek(dataEnd, System.IO.SeekOrigin.Begin);

        foreach (var child in Children)
            child.Write(writer, game);
    }
}