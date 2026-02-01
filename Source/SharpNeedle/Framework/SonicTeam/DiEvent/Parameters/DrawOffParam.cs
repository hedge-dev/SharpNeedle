namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

class DrawOffParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DrawOffParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        Field08 = reader.ReadInt32();
        Field0C = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(Field04);
        writer.WriteInt32(Field08);
        writer.WriteInt32(Field0C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.DrawingOff; }
}