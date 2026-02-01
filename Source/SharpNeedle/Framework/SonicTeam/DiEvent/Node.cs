namespace SharpNeedle.Framework.SonicTeam.DiEvent;

using SharpNeedle.Framework.SonicTeam.DiEvent.NodeData;

public class Node : IBinarySerializable<GameType>
{
    public Guid GUID { get; set; }
    public int Type { get; set; }
    public List<Node> Children { get; set; } = [];
    public uint Flags { get; set; }
    public int Priority { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public string Name { get; set; } = string.Empty;
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
    {
        Read(reader, game);
    }

    public void Read(BinaryObjectReader reader, GameType game)
    {
        GUID = reader.ReadGuid();
        Type = reader.ReadInt32();
        int dataSize = reader.ReadInt32();

        int childCount = reader.ReadInt32();
        Flags = reader.ReadUInt32();

        Priority = reader.ReadInt32();
        Field24 = reader.ReadInt32();
        Field28 = reader.ReadInt32();
        Field2C = reader.ReadInt32();

        Name = reader.ReadDiString(64);

        switch ((NodeType)Type)
        {
            case NodeType.Path:
                Data = reader.ReadObject<PathData, GameType>(game);
                break;

            case NodeType.Camera:
                Data = reader.ReadObject<CameraData, GameType>(game);
                break;

            case NodeType.CameraMotion:
                Data = reader.ReadObject<CameraMotionData, GameType>(game);
                break;

            case NodeType.ModelCustom:
            case NodeType.Character:
                Data = reader.ReadObject<ModelData, GameType>(game);
                break;

            case NodeType.CharacterMotion:
            case NodeType.ModelMotion:
                Data = reader.ReadObject<ModelMotionData, GameType>(game);
                break;

            case NodeType.Attachment:
                Data = reader.ReadObject<AttachmentData, GameType>(game);
                break;

            case NodeType.Parameter:
                ParameterData parameterData = new(dataSize);
                parameterData.Read(reader, game);
                Data = parameterData;
                break;

            default:
                reader.Skip(dataSize * 4);
                break;
        }

        Children.AddRange(reader.ReadObjectArray<Node, GameType>(game, childCount));
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteGuid(GUID);
        writer.WriteInt32(Type);

        long dataSizePos = writer.Position;
        writer.WriteNulls(4);

        writer.WriteInt32(Children.Count);
        writer.WriteUInt32(Flags);

        writer.WriteInt32(Priority);
        writer.WriteInt32(Field24);
        writer.WriteInt32(Field28);
        writer.WriteInt32(Field2C);

        writer.WriteDiString(Name, 64);

        long dataStart = writer.Position;
        Data.Write(writer, game);
        long dataEnd = writer.Position;

        writer.Seek(dataSizePos, SeekOrigin.Begin);
        writer.WriteInt32((int)(dataEnd - dataStart) / 4);
        writer.Seek(dataEnd, SeekOrigin.Begin);

        writer.WriteObjectCollection(game, Children);
    }
}