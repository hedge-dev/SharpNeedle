namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

using SharpNeedle.Structs;

public class ChromaAberrationParam : BaseParam
{
    public Endpoint EndpointA { get; set; } = new Endpoint();
    public float Field20 { get; set; }
    public Endpoint EndpointB { get; set; } = new Endpoint();
    public float[] CurveData { get; set; } = new float[32];

    public ChromaAberrationParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        EndpointA = reader.ReadObject<Endpoint>();
        Field20 = reader.ReadSingle();
        EndpointB = reader.ReadObject<Endpoint>();

        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteObject(EndpointA);
        writer.WriteSingle(Field20);
        writer.WriteObject(EndpointB);

        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ChromaticAberration;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.ChromaticAberration;

            default:
                return 0;
        }
    }

    public class Endpoint : IBinarySerializable
    {
        public Color<float> ColorOffset { get; set; }
        public float SphereCurve { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Position { get; set; }

        public Endpoint() { }

        public void Read(BinaryObjectReader reader)
        {
            ColorOffset = new Color<float>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
            SphereCurve = reader.ReadSingle();
            Scale = reader.ReadVector2();
            Position = reader.ReadVector2();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteVector3(new Vector3(ColorOffset.R, ColorOffset.G, ColorOffset.B));
            writer.WriteSingle(SphereCurve);
            writer.WriteVector2(Scale);
            writer.WriteVector2(Position);
        }
    }
}