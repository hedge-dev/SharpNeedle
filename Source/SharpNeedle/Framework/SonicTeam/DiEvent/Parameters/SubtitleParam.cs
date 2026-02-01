namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class SubtitleParam : BaseParam
{
    public string CellName { get; set; } = string.Empty;
    public SubtitleLanguage Language { get; set; }
    public int Field14 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public string CellName2 { get; set; } = string.Empty;

    public SubtitleParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        if (game == GameType.ShadowGenerations)
        {
            CellName = reader.ReadDiString(32);
            Language = (SubtitleLanguage)reader.ReadInt32();
            Field14 = reader.ReadInt32();
            Field24 = reader.ReadInt32();
            Field28 = reader.ReadInt32();
            CellName2 = reader.ReadDiString(32);
        }
        else
        {
            CellName = reader.ReadDiString(16);
            Language = (SubtitleLanguage)reader.ReadInt32();
            Field14 = reader.ReadInt32();
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        if (game == GameType.ShadowGenerations)
        {
            writer.WriteDiString(CellName, 32);
            writer.WriteInt32((int)Language);
            writer.WriteInt32(Field14);
            writer.WriteInt32(Field24);
            writer.WriteInt32(Field28);
            writer.WriteDiString(CellName2, 32);
        }
        else
        {
            writer.WriteDiString(CellName, 16);
            writer.WriteInt32((int)Language);
            writer.WriteInt32(Field14);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Subtitle;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Subtitle;

            default:
                return 0;
        }
    }

    public enum SubtitleLanguage
    {
        English = 0,
        French = 1,
        Italian = 2,
        German = 3,
        Spanish = 4,
        Polish = 5,
        Portuguese = 6,
        Russian = 7,
        Japanese = 8,
        Chinese = 9,
        ChineseSimplified = 10,
        Korean = 11
    }
}