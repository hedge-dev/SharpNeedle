namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class MovieDisplayParam : BaseParam
{
    public MovieDisplayParam() { }
    public MovieDisplayParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game) { }

    public override void Write(BinaryObjectWriter writer, GameType game) { }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.MovieDisplay;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.MovieDisplay;

            default:
                return 0;
        }
    }
}