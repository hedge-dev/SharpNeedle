namespace SharpNeedle.SonicTeam.DiEvent;

public class ShadowAfterimageParam : BaseParam
{
    public Color<int> Color { get; set; }
    public int Field10 { get; set; } // Possibly afterimage count?
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public int Field2C { get; set; }

    public ShadowAfterimageParam() { }
    public ShadowAfterimageParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Color = reader.Read<Color<int>>();
        Field10 = reader.Read<int>();
        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Color);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.ShadowAfterimage;

            default:
                return 0;
        }
    }
}