namespace SharpNeedle.SonicTeam.DiEvent;

public class VignetteParam : BaseParam
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
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float Field44 { get; set; }
    public float Field48 { get; set; }
    public float Field4C { get; set; }
    public float Field50 { get; set; }
    public float Field54 { get; set; }
    public float Field58 { get; set; }
    public float Field5C { get; set; }
    public float Field60 { get; set; }
    public float Field64 { get; set; }
    public float Field68 { get; set; }
    public float Field6C { get; set; }
    public float Field70 { get; set; }
    public float Field74 { get; set; }
    public float Field78 { get; set; }
    public float Field7C { get; set; }
    public float Field80 { get; set; }
    public float Field84 { get; set; }
    public int Field88 { get; set; }
    public float Field8C { get; set; }
    public float Field90 { get; set; }
    public float Field94 { get; set; }
    public float Field98 { get; set; }
    public float Field9C { get; set; }
    public float FieldA0 { get; set; }
    public float FieldA4 { get; set; }
    public float FieldA8 { get; set; }
    public float FieldAC { get; set; }
    public float FieldB0 { get; set; }
    public float FieldB4 { get; set; }
    public float FieldB8 { get; set; }
    public float FieldBC { get; set; }
    public float FieldC0 { get; set; }
    public float FieldC4 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public VignetteParam() { }
    public VignetteParam(BinaryObjectReader reader, GameType game)
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
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<float>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<float>();
        Field3C = reader.Read<float>();
        Field40 = reader.Read<float>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<float>();
        Field50 = reader.Read<float>();
        Field54 = reader.Read<float>();
        Field58 = reader.Read<float>();
        Field5C = reader.Read<float>();
        Field60 = reader.Read<float>();
        Field64 = reader.Read<float>();
        Field68 = reader.Read<float>();
        Field6C = reader.Read<float>();
        Field70 = reader.Read<float>();
        Field74 = reader.Read<float>();
        Field78 = reader.Read<float>();
        Field7C = reader.Read<float>();
        Field80 = reader.Read<float>();
        Field84 = reader.Read<float>();
        Field88 = reader.Read<int>();
        Field8C = reader.Read<float>();
        Field90 = reader.Read<float>();
        Field94 = reader.Read<float>();
        Field98 = reader.Read<float>();
        Field9C = reader.Read<float>();
        FieldA0 = reader.Read<float>();
        FieldA4 = reader.Read<float>();
        FieldA8 = reader.Read<float>();
        FieldAC = reader.Read<float>();
        FieldB0 = reader.Read<float>();
        FieldB4 = reader.Read<float>();
        FieldB8 = reader.Read<float>();
        FieldBC = reader.Read<float>();
        FieldC0 = reader.Read<float>();
        FieldC4 = reader.Read<float>();

        reader.ReadArray<float>(32, ValuesTimeline);
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
        writer.Write(Field80);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
        writer.Write(Field90);
        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(FieldA0);
        writer.Write(FieldA4);
        writer.Write(FieldA8);
        writer.Write(FieldAC);
        writer.Write(FieldB0);
        writer.Write(FieldB4);
        writer.Write(FieldB8);
        writer.Write(FieldBC);
        writer.Write(FieldC0);
        writer.Write(FieldC4);

        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Vignette;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Vignette;

            default:
                return 0;
        }
    }
}