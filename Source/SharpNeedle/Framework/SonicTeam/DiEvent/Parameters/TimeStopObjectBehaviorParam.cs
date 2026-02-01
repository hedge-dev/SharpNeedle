namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimeStopObjectBehaviorParam : BaseParam
{
    public int Mode { get; set; }

    public TimeStopObjectBehaviorParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Mode = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Mode);
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