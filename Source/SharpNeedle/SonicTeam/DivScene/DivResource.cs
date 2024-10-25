﻿namespace SharpNeedle.SonicTeam.DivScene;

public class DivResource : IBinarySerializable
{
    public Guid GUID { get; set; }
    public bool UnknownFlag { get; set; }
    public ResourceType ResType { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public string Name { get; set; }

    public DivResource() { }

    public DivResource(string name) 
    {
        Name = name;
        GUID = Guid.NewGuid();
    }

    public DivResource(string name, ResourceType type) : this(name)
    {
        ResType = type;
    }

    public void Read(BinaryObjectReader reader)
    {
        GUID = reader.Read<Guid>();

        int resourceFlags = reader.Read<int>();
        UnknownFlag = (resourceFlags & 1) == 1;
        ResType = (ResourceType)(resourceFlags >> 1);

        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 788);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(GUID);
        writer.Write(((int)ResType << 1) | (UnknownFlag ? 1 : 0));
        writer.Write(Field14);
        writer.Write(Field18);
        writer.WriteString(StringBinaryFormat.FixedLength, Name, 788);
    }
}

public enum ResourceType
{
    None = 0,
    AnimationStateMachine = 1,
    CameraAnimation = 2,
    AnimationPxd = 3,
}