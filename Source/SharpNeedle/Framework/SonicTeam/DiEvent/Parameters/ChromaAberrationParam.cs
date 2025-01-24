namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

using SharpNeedle.Structs;

public class ChromaAberrationParam : BaseParam
{
    public Endpoint EndpointA { get; set; } = new Endpoint();
    public float Field20 { get; set; }
    public Endpoint EndpointB { get; set; } = new Endpoint();
    public float[] CurveData { get; set; } = new float[32];

    public ChromaAberrationParam() { }
    public ChromaAberrationParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        EndpointA = reader.ReadObject<Endpoint>();
        Field20 = reader.Read<float>();
        EndpointB = reader.ReadObject<Endpoint>();

        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteObject(EndpointA);
        writer.Write(Field20);
        writer.WriteObject(EndpointB);

        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
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

        public Endpoint(BinaryObjectReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryObjectReader reader)
        {
            ColorOffset = new Color<float>(reader.Read<float>(), reader.Read<float>(), reader.Read<float>(), 1.0f);
            SphereCurve = reader.Read<float>();
            Scale = reader.Read<Vector2>();
            Position = reader.Read<Vector2>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(new Vector3(ColorOffset.R, ColorOffset.G, ColorOffset.B));
            writer.Write(SphereCurve);
            writer.Write(Scale);
            writer.Write(Position);
        }
    }
}