namespace SharpNeedle.Ninja.Chao.Animation;

public class KeyFrameData : IBinarySerializable
{
    public float AnimationLength { get; set; }
    public List<GroupAnimationData> GroupAnimations { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        GroupAnimations = reader.ReadObject<BinaryList<GroupAnimationData>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteObject<BinaryList<GroupAnimationData>>(GroupAnimations);
    }
}