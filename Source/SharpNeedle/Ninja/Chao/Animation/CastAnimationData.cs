namespace SharpNeedle.Ninja.Chao.Animation;

public class CastAnimationData : List<CastAnimationSubData>, IBinarySerializable
{
    public uint Flags { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Clear();
        Flags = reader.Read<uint>();
        var count = BitOperations.PopCount(Flags);

        if (count == 0)
        {
            reader.Skip(reader.GetOffsetSize());
            return;
        }

        AddRange(reader.ReadObjectArrayOffset<CastAnimationSubData>(count));
    }

    public void Write(BinaryObjectWriter writer)
    {
        if (Count == 0)
        {
            writer.Write(0);
            writer.WriteOffsetValue(0);
            return;
        }

        writer.Write(Count << 1);
        writer.WriteObjectCollectionOffset(this);
    }
}

public class CastAnimationSubData : IBinarySerializable
{
    public uint Field00 { get; set; }
    public List<KeyFrame> Frames { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<uint>();
        Frames = new List<KeyFrame>(reader.ReadArrayOffset<KeyFrame>(reader.Read<int>()));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Frames.Count);

        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Frames).AsMemory());
    }
}