namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

using SharpNeedle.Structs;

public class ShadowResolutionParam : BaseParam
{
    public Vector2Int Resolution { get; set; }

    public ShadowResolutionParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Resolution = reader.ReadObject<Vector2Int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteObject(Resolution);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ShadowResolution;

            default:
                return 0;
        }
    }
}