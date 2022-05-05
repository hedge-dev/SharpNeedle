namespace SharpNeedle.Ninja.Cell.Animation;

public class GroupAnimationData : IBinarySerializable
{
    public List<CastAnimationData> CastAnimations { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        CastAnimations = reader.ReadObject<BinaryList<CastAnimationData>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteObject<BinaryList<CastAnimationData>>(CastAnimations);
    }
}