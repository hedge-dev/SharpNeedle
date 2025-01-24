namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;
public class FinalBossLightingParam : BaseParam
{
    public FinalBossLightingParam() { }
    public FinalBossLightingParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game) { }

    public override void Write(BinaryObjectWriter writer, GameType game) { }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.FinalBossLighting;

            default:
                return 0;
        }
    }
}