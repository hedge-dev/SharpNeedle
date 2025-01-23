namespace SharpNeedle.SonicTeam.DiEvent;

public class CyberNoiseParam : BaseParam
{
    public uint Field00 { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public CyberNoiseParam() { }
    public CyberNoiseParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.CyberNoise;

            default:
                return 0;
        }
    }
}
