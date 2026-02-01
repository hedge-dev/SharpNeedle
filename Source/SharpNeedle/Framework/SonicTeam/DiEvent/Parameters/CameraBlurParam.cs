namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CameraBlurParam : BaseParam
{
    public uint Field00 { get; set; }
    public uint Field04 { get; set; }
    public float Field08 { get; set; }
    public float[] CurveData { get; set; } = new float[32];
    public uint Flags { get; set; }

    public CameraBlurParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadUInt32();
        Field04 = reader.ReadUInt32();
        Field08 = reader.ReadSingle();
        reader.ReadArray<float>(32, CurveData);
        Flags = reader.ReadUInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteUInt32(Field00);
        writer.WriteUInt32(Field04);
        writer.WriteSingle(Field08);
        writer.WriteArrayFixedLength(CurveData, 32);
        writer.WriteUInt32(Flags);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.CameraBlur;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.CameraBlur;

            default:
                return 0;
        }
    }
}