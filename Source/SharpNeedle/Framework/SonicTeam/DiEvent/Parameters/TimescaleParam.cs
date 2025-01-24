namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimescaleParam : BaseParam
{
    public int Field00 { get; set; }
    public float Scale { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public TimescaleParam() { }
    public TimescaleParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Scale = reader.Read<float>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Scale);
        writer.Write(Field08);
        writer.Write(Field0C);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
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