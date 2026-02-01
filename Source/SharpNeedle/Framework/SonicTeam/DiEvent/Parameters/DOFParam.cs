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

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        EndpointA = reader.ReadObject<Endpoint>();
        EndpointB = reader.ReadObject<Endpoint>();
        Field24 = reader.ReadSingle();
        Field28 = reader.ReadSingle();
        Field2C = reader.ReadInt32();
        Field30 = reader.ReadInt32();
        Field34 = reader.ReadSingle();
        Field38 = reader.ReadInt32();
        Field3C = reader.ReadInt32();
        Field40 = reader.ReadInt32();
        Field44 = reader.ReadInt32();
        Field48 = reader.ReadInt32();
        Field4C = reader.ReadInt32();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteObject(EndpointA);
        writer.WriteObject(EndpointB);
        writer.WriteSingle(Field24);
        writer.WriteSingle(Field28);
        writer.WriteInt32(Field2C);
        writer.WriteInt32(Field30);
        writer.WriteSingle(Field34);
        writer.WriteInt32(Field38);
        writer.WriteInt32(Field3C);
        writer.WriteInt32(Field40);
        writer.WriteInt32(Field44);
        writer.WriteInt32(Field48);
        writer.WriteInt32(Field4C);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public class Endpoint : IBinarySerializable
    {
        public float Focus { get; set; }
        public float FocusRange { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        public Endpoint() { }

        public void Read(BinaryObjectReader reader)
        {
            Focus = reader.ReadSingle();
            FocusRange = reader.ReadSingle();
            Near = reader.ReadSingle();
            Far = reader.ReadSingle();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteSingle(Focus);
            writer.WriteSingle(FocusRange);
            writer.WriteSingle(Near);
            writer.WriteSingle(Far);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
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