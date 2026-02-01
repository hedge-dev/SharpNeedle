namespace SharpNeedle.Framework.Glitter;

public class AnimationParameter : IBinarySerializable
{
    public List<Keyframe> Keyframes { get; set; } = [];
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        long keyframeOffset = reader.ReadOffsetValue();
        int keyframeCount = reader.ReadInt32();

        reader.ReadAtOffset(keyframeOffset, () =>
        {
            for (int i = 0; i < keyframeCount; i++)
            {
                Keyframes.Add(reader.ReadObject<Keyframe>());
            }
        });

        Field08 = reader.ReadInt32();
        Field0C = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteOffset(() =>
        {
            foreach (Keyframe keyframe in Keyframes)
            {
                writer.WriteObject(keyframe);
            }
        });

        writer.WriteInt32(Keyframes.Count);

        writer.WriteInt32(Field08);
        writer.WriteInt32(Field0C);
    }
}
