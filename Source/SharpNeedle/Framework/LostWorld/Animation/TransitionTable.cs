namespace SharpNeedle.Framework.LostWorld.Animation;

public class TransitionTable : Dictionary<string, string>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        Clear();
        int count = reader.Read<int>();
        reader.ReadOffset(() =>
        {
            for(int i = 0; i < count; i++)
            {
                Add(reader.ReadStringOffset(), reader.ReadStringOffset());
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Count);

        writer.WriteOffset(() =>
        {
            List<KeyValuePair<string, string>> transitions = [.. this];
            transitions.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.InvariantCulture));

            foreach(KeyValuePair<string, string> pair in transitions)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, pair.Key);
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, pair.Value);
            }
        });
    }
}