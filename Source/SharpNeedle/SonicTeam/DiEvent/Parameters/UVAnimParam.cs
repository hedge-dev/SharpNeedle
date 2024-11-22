namespace SharpNeedle.SonicTeam.DiEvent;

public class UVAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string Name { get; set; }
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public UVAnimParam() { }
    public UVAnimParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDiString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.UVAnimation; }
}