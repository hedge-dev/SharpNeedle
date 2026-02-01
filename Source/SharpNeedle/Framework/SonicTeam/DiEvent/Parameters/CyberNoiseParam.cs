namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CyberNoiseParam : BaseParam
{
    public uint Field00 { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public CyberNoiseParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadUInt32();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteUInt32(Field00);
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