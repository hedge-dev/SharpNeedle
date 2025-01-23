namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CompositeAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string StateName { get; set; }
    public Animation[] Animations { get; set; } = new Animation[16];
    public int ActiveAnimCount { get; set; }

    public CompositeAnimParam() { }
    public CompositeAnimParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        StateName = reader.ReadString(StringBinaryFormat.FixedLength, 12);
        Animations = reader.ReadObjectArray<Animation>(16);
        ActiveAnimCount = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, StateName, 12);
        writer.WriteObjectCollection(Animations);
        writer.Write(ActiveAnimCount);
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
            Type = reader.Read<int>();
            Name = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Type);
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