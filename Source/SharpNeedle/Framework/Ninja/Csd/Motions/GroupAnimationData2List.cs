namespace SharpNeedle.Framework.Ninja.Csd.Motions;

public class GroupAnimationData2List : IBinarySerializable<Family>
{
    public Family? Target { get; set; }

    public void Read(BinaryObjectReader reader, Family context)
    {
        Target = context;
    }

    public void Write(BinaryObjectWriter writer, Family context)
    {
        throw new NotImplementedException();
    }
}