namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimeStopObjectBehaviorParam : BaseParam
{
    public int Mode { get; set; }

    public TimeStopObjectBehaviorParam() { }
    public TimeStopObjectBehaviorParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Mode = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Mode);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStopObjectBehavior;

            default:
                return 0;
        }
    }
}