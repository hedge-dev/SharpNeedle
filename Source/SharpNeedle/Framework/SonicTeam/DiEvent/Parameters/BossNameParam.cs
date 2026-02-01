namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class BossNameParam : BaseParam
{
    public int Field00 { get; set; }
    public int NameType { get; set; }

    public BossNameParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        NameType = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(NameType);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.BossName;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.BossName;

            default:
                return 0;
        }
    }

    public enum FrontiersNames
    {
        Giant = 0,
        Dragon,
        Knight,
        Rifle,
        TheEnd,
        RifleBeast
    }

    public enum ShadowGensNames
    {
        Biolizard = 0,
        MetalOverlord,
        Mephiles,
        DevilDoom,
        PerfectBlackDoom
    }
}