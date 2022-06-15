namespace SharpNeedle.SurfRide.Draw.Animation;

public class KeyIndividual : IKeyFrame
{
    public int Frame { get; set; }
    public Union Value { get; set; }
    public float TangentIn { get; set; }
    public float TangentOut { get; set; }
    public int Field10 { get; set; }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        Frame = reader.Read<int>();
        Value = reader.ReadObject<Union, uint>(flags);
        TangentIn = reader.Read<float>();
        TangentOut = reader.Read<float>();
        Field10 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, uint flags)
    {
        writer.Write(Frame);
        writer.WriteObject(Value, flags);
        writer.Write(TangentIn);
        writer.Write(TangentOut);
        writer.Write(Field10);
    }
}
