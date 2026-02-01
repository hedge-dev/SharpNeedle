namespace SharpNeedle.Framework.LostWorld.Animation;

public struct PlayModeInfo : IBinarySerializable
{
    public PlayMode Mode;
    public short RandomMin;
    public short RandomMax;

    public void Read(BinaryObjectReader reader)
    {
        Mode = (PlayMode)reader.ReadInt32();
        RandomMin = reader.ReadInt16();
        RandomMax = reader.ReadInt16();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)Mode);
        writer.WriteInt16(RandomMin);
        writer.WriteInt16(RandomMax);
    }
}

public enum PlayMode
{
    Loop, Stop
}