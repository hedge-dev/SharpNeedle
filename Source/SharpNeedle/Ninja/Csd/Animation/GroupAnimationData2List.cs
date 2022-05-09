namespace SharpNeedle.Ninja.Csd.Animation;

public class GroupAnimationData2List : IBinarySerializable<Layer>
{
    public Layer Target { get; set; }
    public void Read(BinaryObjectReader reader, Layer context)
    {
        Target = context;
    }

    public void Write(BinaryObjectWriter writer, Layer context)
    {
        throw new NotImplementedException();
    }
}