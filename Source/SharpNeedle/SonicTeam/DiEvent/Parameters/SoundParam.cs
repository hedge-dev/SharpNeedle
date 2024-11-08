namespace SharpNeedle.SonicTeam.DiEvent;

public class SoundParam : BaseParam
{
    public string CueName { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }

    public SoundParam() { }
    public SoundParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        CueName = reader.ReadDiString(64);
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteDiString(CueName);
        writer.Write(Field40);
        writer.Write(Field44);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Sound;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Sound;

            default:
                return 0;
        }
    }
}