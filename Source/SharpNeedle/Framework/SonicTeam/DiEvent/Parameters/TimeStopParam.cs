namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimeStopParam : BaseParam
{
    public int Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }

    public TimeStopParam() { }
    public TimeStopParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStop;

            default:
                return 0;
        }
    }

    public enum ParamType
    {
        Float = 3
    }
}