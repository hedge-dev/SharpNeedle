namespace SharpNeedle.SonicTeam.DiEvent;

public class SubtitleParam : BaseParam
{
    public string CellName { get; set; }
    public SubtitleLanguage Language { get; set; }
    public int Field14 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public string CellName2 { get; set; }

    public SubtitleParam() { }
    public SubtitleParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        if (game == GameType.ShadowGenerations)
        {
            CellName = reader.ReadDiString(32);
            Language = (SubtitleLanguage)reader.Read<int>();
            Field14 = reader.Read<int>();
            Field24 = reader.Read<int>();
            Field28 = reader.Read<int>();
            CellName2 = reader.ReadDiString(32);
        }
        else
        {
            CellName = reader.ReadDiString(16);
            Language = (SubtitleLanguage)reader.Read<int>();
            Field14 = reader.Read<int>();
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        if (game == GameType.ShadowGenerations)
        {
            writer.WriteDiString(CellName, 32);
            writer.Write((int)Language);
            writer.Write(Field14);
            writer.Write(Field24);
            writer.Write(Field28);
            writer.WriteDiString(CellName2, 32);
        }
        else
        {
            writer.WriteDiString(CellName, 16);
            writer.Write((int)Language);
            writer.Write(Field14);
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