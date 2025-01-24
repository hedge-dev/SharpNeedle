namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class DOFParam : BaseParam
{
    public int Field00 { get; set; }
    public Endpoint EndpointA { get; set; } = new Endpoint();
    public Endpoint EndpointB { get; set; } = new Endpoint();
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public DOFParam() { }
    public DOFParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        EndpointA = reader.ReadObject<Endpoint>();
        EndpointB = reader.ReadObject<Endpoint>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteObject(EndpointA);
        writer.WriteObject(EndpointB);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public class Endpoint : IBinarySerializable
    {
        public float Focus { get; set; }
        public float FocusRange { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        public Endpoint() { }

        public Endpoint(BinaryObjectReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryObjectReader reader)
        {
            Focus = reader.Read<float>();
            FocusRange = reader.Read<float>();
            Near = reader.Read<float>();
            Far = reader.Read<float>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Focus);
            writer.Write(FocusRange);
            writer.Write(Near);
            writer.Write(Far);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.DepthOfField;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.DepthOfField;

            default:
                return 0;
        }
    }
}