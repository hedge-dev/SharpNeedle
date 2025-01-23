namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class LetterboxParam : BaseParam
{
    public float[] CurveData { get; set; } = new float[32];

    public LetterboxParam() { }
    public LetterboxParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Letterbox;

            default:
                return 0;
        }
    }
}
