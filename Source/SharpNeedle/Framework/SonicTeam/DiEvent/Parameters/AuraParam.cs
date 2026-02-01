namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class AuraParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float Field44 { get; set; }
    public int Field48 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public AuraParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        Field08 = reader.ReadInt32();
        Field0C = reader.ReadInt32();
        Field10 = reader.ReadSingle();
        Field14 = reader.ReadSingle();
        Field18 = reader.ReadSingle();
        Field1C = reader.ReadSingle();
        Field20 = reader.ReadSingle();
        Field24 = reader.ReadInt32();
        Field28 = reader.ReadInt32();
        Field2C = reader.ReadInt32();
        Field30 = reader.ReadInt32();
        Field34 = reader.ReadSingle();
        Field38 = reader.ReadSingle();
        Field3C = reader.ReadSingle();
        Field40 = reader.ReadSingle();
        Field44 = reader.ReadSingle();
        Field48 = reader.ReadInt32();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(Field04);
        writer.WriteInt32(Field08);
        writer.WriteInt32(Field0C);
        writer.WriteSingle(Field10);
        writer.WriteSingle(Field14);
        writer.WriteSingle(Field18);
        writer.WriteSingle(Field1C);
        writer.WriteSingle(Field20);
        writer.WriteInt32(Field24);
        writer.WriteInt32(Field28);
        writer.WriteInt32(Field2C);
        writer.WriteInt32(Field30);
        writer.WriteSingle(Field34);
        writer.WriteSingle(Field38);
        writer.WriteSingle(Field3C);
        writer.WriteSingle(Field40);
        writer.WriteSingle(Field44);
        writer.WriteInt32(Field48);
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Aura;

            case GameType.ShadowGenerations:
                return 0;

            default:
                return 0;
        }
    }
}