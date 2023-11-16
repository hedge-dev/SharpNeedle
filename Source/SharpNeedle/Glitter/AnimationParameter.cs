namespace SharpNeedle.Glitter;

public class AnimationParameter : IBinarySerializable
{
    public List<Keyframe> Keyframes { get; set; } = new();
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        long keyframeOffset = reader.ReadOffsetValue();
        int keyframeCount = reader.Read<int>();

        reader.ReadAtOffset(keyframeOffset, () =>
        {
            for (int i = 0; i < keyframeCount; i++)
                Keyframes.Add(reader.ReadObject<Keyframe>());
        });

        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteOffset(() =>
        {
            foreach (var keyframe in Keyframes)
                writer.WriteObject(keyframe);
        });

        writer.Write(Keyframes.Count);

        writer.Write(Field08);
        writer.Write(Field0C);
    }
}
