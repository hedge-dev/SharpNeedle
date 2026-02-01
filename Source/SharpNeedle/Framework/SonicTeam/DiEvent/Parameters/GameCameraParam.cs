namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class GameCameraParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public float Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public float NearCullingPlane { get; set; }
    public float FarCullingPlane { get; set; }
    public float Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public int Field64 { get; set; }

    public GameCameraParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        Field08 = reader.ReadInt32();
        Field0C = reader.ReadInt32();
        Field10 = reader.ReadSingle();
        Field14 = reader.ReadSingle();
        Field18 = reader.ReadSingle();
        Field1C = reader.ReadInt32();
        Field20 = reader.ReadInt32();
        Field24 = reader.ReadInt32();
        Field28 = reader.ReadSingle();
        Field2C = reader.ReadSingle();
        Field30 = reader.ReadSingle();
        Field34 = reader.ReadInt32();
        Field38 = reader.ReadInt32();
        Field3C = reader.ReadInt32();
        Field40 = reader.ReadInt32();
        Field44 = reader.ReadSingle();
        Field48 = reader.ReadInt32();
        Field4C = reader.ReadInt32();
        NearCullingPlane = reader.ReadSingle();
        FarCullingPlane = reader.ReadSingle();
        Field58 = reader.ReadSingle();
        Field5C = reader.ReadInt32();
        Field60 = reader.ReadInt32();
        Field64 = reader.ReadInt32();
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
        writer.WriteInt32(Field1C);
        writer.WriteInt32(Field20);
        writer.WriteInt32(Field24);
        writer.WriteSingle(Field28);
        writer.WriteSingle(Field2C);
        writer.WriteSingle(Field30);
        writer.WriteInt32(Field34);
        writer.WriteInt32(Field38);
        writer.WriteInt32(Field3C);
        writer.WriteInt32(Field40);
        writer.WriteSingle(Field44);
        writer.WriteInt32(Field48);
        writer.WriteInt32(Field4C);
        writer.WriteSingle(NearCullingPlane);
        writer.WriteSingle(FarCullingPlane);
        writer.WriteSingle(Field58);
        writer.WriteInt32(Field5C);
        writer.WriteInt32(Field60);
        writer.WriteInt32(Field64);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.GameCamera; }
}