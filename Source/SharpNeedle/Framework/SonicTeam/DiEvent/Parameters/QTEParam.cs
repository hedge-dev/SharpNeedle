namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class QTEParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public int Field64 { get; set; }
    public int Field68 { get; set; }
    public int Field6C { get; set; }
    public float Field70 { get; set; }
    public float Field74 { get; set; }
    public float Field78 { get; set; }
    public float Field7C { get; set; }
    public int[] Field80 { get; set; } = new int[32];
    public string Field100 { get; set; }
    public string Field140 { get; set; }
    public string Field180 { get; set; }
    public uint Field1C0 { get; set; }

    public QTEParam() { }
    public QTEParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<int>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
        Field54 = reader.Read<int>();
        Field58 = reader.Read<int>();
        Field5C = reader.Read<int>();
        Field60 = reader.Read<int>();
        Field64 = reader.Read<int>();
        Field68 = reader.Read<int>();
        Field6C = reader.Read<int>();
        Field70 = reader.Read<float>();
        Field74 = reader.Read<float>();
        Field78 = reader.Read<float>();
        Field7C = reader.Read<float>();
        reader.ReadArray<int>(32, Field80);

        Field100 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field140 = reader.ReadString(StringBinaryFormat.FixedLength, 64);

        if(game == GameType.ShadowGenerations)
        {
            Field180 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
            Field1C0 = reader.Read<uint>();
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
        writer.Write(Field54);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);
        writer.Write(Field64);
        writer.Write(Field68);
        writer.Write(Field6C);
        writer.Write(Field70);
        writer.Write(Field74);
        writer.Write(Field78);
        writer.Write(Field7C);
        writer.WriteArrayFixedLength(Field80, 32);

        writer.WriteString(StringBinaryFormat.FixedLength, Field100, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Field140, 64);

        if(game == GameType.ShadowGenerations)
        {
            writer.WriteString(StringBinaryFormat.FixedLength, Field180, 64);
            writer.Write(Field1C0);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.QTE;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.QTE;

            default:
                return 0;
        }
    }
}