namespace SharpNeedle.SurfRide.Draw.Animation;

public class KeyHermite : IKeyFrame
{
    public int Frame { get; set; }
    public Union Value { get; set; }
    public float TangentIn { get; set; }
    public float TangentOut { get; set; }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        Frame = reader.Read<int>();
        Value = reader.ReadObject<Union, uint>(flags);
        TangentIn = reader.Read<float>();
        TangentOut = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer, uint flags)
    {
        writer.Write(Frame);
        writer.WriteObject(Value, flags);
        writer.Write(TangentIn);
        writer.Write(TangentOut);
    }
}
