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
    public string Field100 { get; set; } = string.Empty;
    public string Field140 { get; set; } = string.Empty;
    public string Field180 { get; set; } = string.Empty;
    public uint Field1C0 { get; set; }

    public QTEParam() { }

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
        Field28 = reader.ReadInt32();
        Field2C = reader.ReadInt32();
        Field30 = reader.ReadInt32();
        Field34 = reader.ReadInt32();
        Field38 = reader.ReadInt32();
        Field3C = reader.ReadInt32();
        Field40 = reader.ReadInt32();
        Field44 = reader.ReadInt32();
        Field48 = reader.ReadInt32();
        Field4C = reader.ReadInt32();
        Field50 = reader.ReadInt32();
        Field54 = reader.ReadInt32();
        Field58 = reader.ReadInt32();
        Field5C = reader.ReadInt32();
        Field60 = reader.ReadInt32();
        Field64 = reader.ReadInt32();
        Field68 = reader.ReadInt32();
        Field6C = reader.ReadInt32();
        Field70 = reader.ReadSingle();
        Field74 = reader.ReadSingle();
        Field78 = reader.ReadSingle();
        Field7C = reader.ReadSingle();
        reader.ReadArray<int>(32, Field80);

        Field100 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field140 = reader.ReadString(StringBinaryFormat.FixedLength, 64);

        if (game == GameType.ShadowGenerations)
        {
            Field180 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
            Field1C0 = reader.ReadUInt32();
        }
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
        writer.WriteInt32(Field28);
        writer.WriteInt32(Field2C);
        writer.WriteInt32(Field30);
        writer.WriteInt32(Field34);
        writer.WriteInt32(Field38);
        writer.WriteInt32(Field3C);
        writer.WriteInt32(Field40);
        writer.WriteInt32(Field44);
        writer.WriteInt32(Field48);
        writer.WriteInt32(Field4C);
        writer.WriteInt32(Field50);
        writer.WriteInt32(Field54);
        writer.WriteInt32(Field58);
        writer.WriteInt32(Field5C);
        writer.WriteInt32(Field60);
        writer.WriteInt32(Field64);
        writer.WriteInt32(Field68);
        writer.WriteInt32(Field6C);
        writer.WriteSingle(Field70);
        writer.WriteSingle(Field74);
        writer.WriteSingle(Field78);
        writer.WriteSingle(Field7C);
        writer.WriteArrayFixedLength(Field80, 32);

        writer.WriteString(StringBinaryFormat.FixedLength, Field100, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Field140, 64);

        if (game == GameType.ShadowGenerations)
        {
            writer.WriteString(StringBinaryFormat.FixedLength, Field180, 64);
            writer.WriteUInt32(Field1C0);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
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