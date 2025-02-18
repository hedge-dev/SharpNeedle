namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;
public class DitherDepthParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }

    public DitherDepthParam() { }
    public DitherDepthParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.DitherDepth;

            default:
                return 0;
        }
    }
}