namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;
public class DitherDepthParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }

    public DitherDepthParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadUInt32();
        Field04 = reader.ReadSingle();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteUInt32(Field00);
        writer.WriteSingle(Field04);
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