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
            var transitions = this.ToList();
            transitions.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.InvariantCulture));

            foreach (var pair in transitions)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, pair.Key);
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, pair.Value);
            }
        });
    }
}