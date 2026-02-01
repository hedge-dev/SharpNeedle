namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class SoundParam : BaseParam
{
    public string CueName { get; set; } = string.Empty;
    public int Field40 { get; set; }
    public int Field44 { get; set; }

    public SoundParam() { }


    public override void Read(BinaryObjectReader reader, GameType game)
    {
        CueName = reader.ReadDiString(64);
        Field40 = reader.ReadInt32();
        Field44 = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteDiString(CueName);
        writer.WriteInt32(Field40);
        writer.WriteInt32(Field44);
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