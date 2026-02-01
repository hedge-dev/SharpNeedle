namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CameraShakeParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public int Field10 { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }

    public CameraShakeParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        Field08 = reader.ReadSingle();
        Field0C = reader.ReadSingle();
        Field10 = reader.ReadInt32();
        Field14 = reader.ReadInt32();
        Field18 = reader.ReadInt32();
        Field1C = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(Field04);
        writer.WriteSingle(Field08);
        writer.WriteSingle(Field0C);
        writer.WriteInt32(Field10);
        writer.WriteInt32(Field14);
        writer.WriteInt32(Field18);
        writer.WriteInt32(Field1C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CameraShake; }
}