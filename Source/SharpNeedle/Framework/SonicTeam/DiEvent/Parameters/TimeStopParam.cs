namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TimeStopParam : BaseParam
{
    public int Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }

    public TimeStopParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadSingle();
        Field08 = reader.ReadSingle();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteSingle(Field04);
        writer.WriteSingle(Field08);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
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