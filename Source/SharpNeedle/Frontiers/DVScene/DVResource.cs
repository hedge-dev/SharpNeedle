namespace SharpNeedle.Frontiers.DVScene;

public class DVResource : IBinarySerializable
{
    public DVGuid GUID { get; set; }
    public bool IsRenderable { get; set; }
    public ResourceType ResType { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public string Name { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        GUID = reader.Read<DVGuid>();

        int resourceFlags = reader.Read<int>();
        IsRenderable = (resourceFlags & 1) == 1;
        ResType = (ResourceType)(resourceFlags >> 1);

        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Name = reader.ReadDVString(64);

        reader.Skip(724);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(GUID);
        writer.Write(((int)ResType << 1) | (IsRenderable ? 1 : 0));
        writer.Write(Field14);
        writer.Write(Field18);
        writer.WriteDVString(Name, 64);

        writer.Skip(724);
    }
}

public enum ResourceType
{
    None = 0,
    AnimationStateMachine = 1,
    CameraAnimation = 2,
    AnimationPxd = 3,
}