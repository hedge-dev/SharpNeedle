namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class ColorCorrectionParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public uint Field14 { get; set; }
    public float Field18 { get; set; }
    public uint Field1C { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public ColorCorrectionParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadUInt32();
        Field04 = reader.ReadSingle();
        Field08 = reader.ReadSingle();
        Field0C = reader.ReadSingle();
        Field10 = reader.ReadSingle();
        Field14 = reader.ReadUInt32();
        Field18 = reader.ReadSingle();
        Field1C = reader.ReadUInt32();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteUInt32(Field00);
        writer.WriteSingle(Field04);
        writer.WriteSingle(Field08);
        writer.WriteSingle(Field0C);
        writer.WriteSingle(Field10);
        writer.WriteUInt32(Field14);
        writer.WriteSingle(Field18);
        writer.WriteUInt32(Field1C);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ColorCorrection;

            default:
                return 0;
        }
    }
}