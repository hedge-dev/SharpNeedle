namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CameraShakeLoopParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float[] CurveData { get; set; } = new float[64];

    public CameraShakeLoopParam() { }

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
        reader.ReadArray<float>(64, CurveData);
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
        writer.WriteArrayFixedLength(CurveData, 64);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CameraShakeLoop; }
}