namespace SharpNeedle.SonicTeam.DiEvent;

public class TimeStopObjectBehaviorParam : BaseParam
{
    public int Mode { get; set; }

    public TimeStopObjectBehaviorParam() { }
    public TimeStopObjectBehaviorParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Mode = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Mode);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStopObjectBehavior;

            default:
                return 0;
        }
    }
}