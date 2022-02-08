namespace SharpNeedle.LostWorld.Animation;

public class TransitionTable : Dictionary<string, string>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        Clear();
        var count = reader.Read<int>();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
                Add(reader.ReadStringOffset(), reader.ReadStringOffset());
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Count);

        writer.WriteOffset(() =>
        {
            foreach (var pair in this)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, pair.Key);
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, pair.Value);
            }
        });
    }
}