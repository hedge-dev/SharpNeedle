namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimeStopControlParam : BaseParam
{
    public int Behavior { get; set; }
    public float Field04 { get; set; }
    public float TransitionDuration { get; set; }

    public TimeStopControlParam() { }
    public TimeStopControlParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Behavior = reader.Read<int>();
        Field04 = reader.Read<float>();
        TransitionDuration = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Behavior);
        writer.Write(Field04);
        writer.Write(TransitionDuration);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStopControl;

            default:
                return 0;
        }
    }

    public enum BehaviorMode
    {
        End = 1,
        Begin = 2,
    }
}