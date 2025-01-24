namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class FalloffToggleParam : BaseParam
{
    public float Intensity { get; set; }

    public FalloffToggleParam() { }
    public FalloffToggleParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Intensity = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Intensity);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.FalloffToggle;

            default:
                return 0;
        }
    }
}