namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CameraExposureParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }
    public uint Field14 { get; set; }
    public uint Field18 { get; set; }
    public uint Field1C { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public CameraExposureParam() { }
    public CameraExposureParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<uint>();
        Field1C = reader.Read<uint>();
        reader.ReadArray<float>(32, CurveData);
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
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.CameraExposure;

            default:
                return 0;
        }
    }
}