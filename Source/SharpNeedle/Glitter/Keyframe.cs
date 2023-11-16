namespace SharpNeedle.Glitter;

public class Keyframe : IBinarySerializable
{
    public enum EInterpolationType : int
    {
        Linear = 1,
        Hermite
    };

    public int Frame { get; set; }
    public float Value { get; set; }
    public float In { get; set; }
    public float Out { get; set; }
    public float Random { get; set; }
    public EInterpolationType InterpolationType { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Frame = reader.Read<int>();
        Value = reader.Read<float>();

        InterpolationType = reader.Read<EInterpolationType>();

        In = reader.Read<float>();
        Out = reader.Read<float>();

        Random = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Frame);
        writer.Write(Value);

        writer.Write(InterpolationType);

        writer.Write(In);
        writer.Write(Out);

        writer.Write(Random);
    }
}
