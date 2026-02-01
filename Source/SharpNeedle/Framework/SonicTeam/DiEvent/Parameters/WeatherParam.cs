namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class WeatherParam : BaseParam
{
    public uint Field00 { get; set; }
    public float[] DataCurve { get; set; } = new float[32];

    public WeatherParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadUInt32();
        reader.ReadArray<float>(32, DataCurve);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteUInt32(Field00);
        writer.WriteArrayFixedLength(DataCurve, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Weather;

            default:
                return 0;
        }
    }
}