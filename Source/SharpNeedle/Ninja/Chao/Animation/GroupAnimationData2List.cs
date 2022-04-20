namespace SharpNeedle.Ninja.Chao.Animation;

public class GroupAnimationData2List : IBinarySerializable<Group>
{
    public Group Target { get; set; }
    public void Read(BinaryObjectReader reader, Group context)
    {
        Target = context;
    }

    public void Write(BinaryObjectWriter writer, Group context)
    {
        throw new NotImplementedException();
    }
}