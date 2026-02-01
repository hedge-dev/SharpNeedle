namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CompositeAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string StateName { get; set; } = string.Empty;
    public Animation[] Animations { get; set; } = new Animation[16];
    public int ActiveAnimCount { get; set; }

    public CompositeAnimParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        StateName = reader.ReadString(StringBinaryFormat.FixedLength, 12);
        Animations = reader.ReadObjectArray<Animation>(16);
        ActiveAnimCount = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, StateName, 12);
        writer.WriteObjectCollection(Animations);
        writer.WriteInt32(ActiveAnimCount);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CompositeAnimation; }

    public class Animation : IBinarySerializable
    {
        public int Type { get; set; }
        public string Name { get; set; }

        public Animation()
        {
            Type = (int)AnimationType.None;
            Name = "";
        }

        public void Read(BinaryObjectReader reader)
        {
            Type = reader.ReadInt32();
            Name = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteInt32(Type);
            writer.WriteString(StringBinaryFormat.FixedLength, Name, 64);
        }
    }

    public enum AnimationType
    {
        None = 0,
        PXD = 1,
        UV = 2,
        Visibility = 3,
        Material = 4,
    }
}