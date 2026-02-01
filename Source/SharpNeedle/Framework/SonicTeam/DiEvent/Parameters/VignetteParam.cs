namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

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

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        Field08 = reader.ReadSingle();
        Field0C = reader.ReadSingle();
        Field10 = reader.ReadSingle();
        Field14 = reader.ReadSingle();
        Field18 = reader.ReadSingle();
        Field1C = reader.ReadSingle();
        Field20 = reader.ReadSingle();
        Field24 = reader.ReadInt32();
        Field28 = reader.ReadSingle();
        Field2C = reader.ReadSingle();
        Field30 = reader.ReadSingle();
        Field34 = reader.ReadSingle();
        Field38 = reader.ReadSingle();
        Field3C = reader.ReadSingle();
        Field40 = reader.ReadSingle();
        Field44 = reader.ReadSingle();
        Field48 = reader.ReadSingle();
        Field4C = reader.ReadSingle();
        Field50 = reader.ReadSingle();
        Field54 = reader.ReadSingle();
        Field58 = reader.ReadSingle();
        Field5C = reader.ReadSingle();
        Field60 = reader.ReadSingle();
        Field64 = reader.ReadSingle();
        Field68 = reader.ReadSingle();
        Field6C = reader.ReadSingle();
        Field70 = reader.ReadSingle();
        Field74 = reader.ReadSingle();
        Field78 = reader.ReadSingle();
        Field7C = reader.ReadSingle();
        Field80 = reader.ReadSingle();
        Field84 = reader.ReadSingle();
        Field88 = reader.ReadInt32();
        Field8C = reader.ReadSingle();
        Field90 = reader.ReadSingle();
        Field94 = reader.ReadSingle();
        Field98 = reader.ReadSingle();
        Field9C = reader.ReadSingle();
        FieldA0 = reader.ReadSingle();
        FieldA4 = reader.ReadSingle();
        FieldA8 = reader.ReadSingle();
        FieldAC = reader.ReadSingle();
        FieldB0 = reader.ReadSingle();
        FieldB4 = reader.ReadSingle();
        FieldB8 = reader.ReadSingle();
        FieldBC = reader.ReadSingle();
        FieldC0 = reader.ReadSingle();
        FieldC4 = reader.ReadSingle();

        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(Field04);
        writer.WriteSingle(Field08);
        writer.WriteSingle(Field0C);
        writer.WriteSingle(Field10);
        writer.WriteSingle(Field14);
        writer.WriteSingle(Field18);
        writer.WriteSingle(Field1C);
        writer.WriteSingle(Field20);
        writer.WriteInt32(Field24);
        writer.WriteSingle(Field28);
        writer.WriteSingle(Field2C);
        writer.WriteSingle(Field30);
        writer.WriteSingle(Field34);
        writer.WriteSingle(Field38);
        writer.WriteSingle(Field3C);
        writer.WriteSingle(Field40);
        writer.WriteSingle(Field44);
        writer.WriteSingle(Field48);
        writer.WriteSingle(Field4C);
        writer.WriteSingle(Field50);
        writer.WriteSingle(Field54);
        writer.WriteSingle(Field58);
        writer.WriteSingle(Field5C);
        writer.WriteSingle(Field60);
        writer.WriteSingle(Field64);
        writer.WriteSingle(Field68);
        writer.WriteSingle(Field6C);
        writer.WriteSingle(Field70);
        writer.WriteSingle(Field74);
        writer.WriteSingle(Field78);
        writer.WriteSingle(Field7C);
        writer.WriteSingle(Field80);
        writer.WriteSingle(Field84);
        writer.WriteInt32(Field88);
        writer.WriteSingle(Field8C);
        writer.WriteSingle(Field90);
        writer.WriteSingle(Field94);
        writer.WriteSingle(Field98);
        writer.WriteSingle(Field9C);
        writer.WriteSingle(FieldA0);
        writer.WriteSingle(FieldA4);
        writer.WriteSingle(FieldA8);
        writer.WriteSingle(FieldAC);
        writer.WriteSingle(FieldB0);
        writer.WriteSingle(FieldB4);
        writer.WriteSingle(FieldB8);
        writer.WriteSingle(FieldBC);
        writer.WriteSingle(FieldC0);
        writer.WriteSingle(FieldC4);

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