namespace SharpNeedle.Framework.Glitter;

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
        Frame = reader.ReadInt32();
        Value = reader.ReadSingle();

        InterpolationType = (EInterpolationType)reader.ReadInt32();

        In = reader.ReadSingle();
        Out = reader.ReadSingle();

        Random = reader.ReadSingle();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Frame);
        writer.WriteSingle(Value);

        writer.WriteInt32((int)InterpolationType);

        writer.WriteSingle(In);
        writer.WriteSingle(Out);

        writer.WriteSingle(Random);
    }
}
