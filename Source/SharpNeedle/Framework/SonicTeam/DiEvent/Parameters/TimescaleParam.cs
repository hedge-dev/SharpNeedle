namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimescaleParam : BaseParam
{
    public int Field00 { get; set; }
    public float Scale { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public TimescaleParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Scale = reader.ReadSingle();
        Field08 = reader.ReadInt32();
        Field0C = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteSingle(Scale);
        writer.WriteInt32(Field08);
        writer.WriteInt32(Field0C);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.TimescaleChange;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimescaleChange;

            default:
                return 0;
        }
    }
}