namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

using SharpNeedle.Structs;

public class FadeParam : BaseParam
{
    Color<uint> Color { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public FadeParam() { }
    public FadeParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Color = reader.Read<Color<uint>>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Color);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Fade;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Fade;

            default:
                return 0;
        }
    }
}