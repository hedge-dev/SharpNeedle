namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimeStopControlParam : BaseParam
{
    public int Behavior { get; set; }
    public float Field04 { get; set; }
    public float TransitionDuration { get; set; }

    public TimeStopControlParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Behavior = reader.ReadInt32();
        Field04 = reader.ReadSingle();
        TransitionDuration = reader.ReadSingle();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Behavior);
        writer.WriteSingle(Field04);
        writer.WriteSingle(TransitionDuration);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
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