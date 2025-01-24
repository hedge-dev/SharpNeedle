namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class TheEndCableParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float[] Field08 { get; set; } = new float[1024];

    public TheEndCableParam() { }
    public TheEndCableParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        reader.ReadArray<float>(1024, Field08);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.WriteArrayFixedLength(Field08, 1024);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.TheEndCable;

            default:
                return 0;
        }
    }
}