namespace SharpNeedle.SurfRide.Draw.Animation;

public class KeyLinear : IKeyFrame
{
    public int Frame { get; set; }
    public Union Value { get; set; }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        Frame = reader.Read<int>();
        Value = reader.ReadObject<Union, uint>(flags);
    }

    public void Write(BinaryObjectWriter writer, uint flags)
    {
        writer.Write(Frame);
        writer.WriteObject(Value, flags);
    }
}
