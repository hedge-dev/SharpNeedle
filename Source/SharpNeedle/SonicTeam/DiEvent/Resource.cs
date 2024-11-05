namespace SharpNeedle.SonicTeam.DiEvent;

public class Resource : IBinarySerializable
{
    public Guid GUID { get; set; }
    public ResourceType ResType { get; set; }
    public uint Flags { get; set; }
    public int Field18 { get; set; }
    public string Name { get; set; }

    public Resource() { }

    public Resource(string name) 
    {
        Name = name;
        GUID = Guid.NewGuid();
    }

    public Resource(string name, ResourceType type) : this(name)
    {
        ResType = type;
    }

    public void Read(BinaryObjectReader reader)
    {
        GUID = reader.Read<Guid>();
        ResType = (ResourceType)reader.Read<int>();
        Flags = reader.Read<uint>();
        Field18 = reader.Read<int>();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 788);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(GUID);
        writer.Write((int)ResType);
        writer.Write(Flags);
        writer.Write(Field18);
        writer.WriteString(StringBinaryFormat.FixedLength, Name, 788);
    }
}

public enum ResourceType
{
    None = 0,
    Texture = 1,
    Character = 2,
    Model = 3,
    MotionCamera = 4,
    MotionPath = 5,
    MotionModel = 6,
    MotionCharacter = 7,
    EquipModel = 8,
    Behavior = 9,
}