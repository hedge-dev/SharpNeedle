namespace SharpNeedle.Ninja.Chao;

public class CastDictionary : List<(string Name, int GroupIdx, int CastIdx)>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        var count = reader.Read<int>();
        if (count == 0)
        {
            reader.Skip(reader.GetOffsetSize());
            return;
        }

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
            {
                Add(new(reader.ReadStringOffset(), reader.Read<int>(), reader.Read<int>()));
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Count);
        if (Count == 0)
        {
            writer.WriteOffsetValue(0);
            return;
        }

        writer.WriteOffset(() =>
        {
            foreach (var item in this)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, item.Name);
                writer.Write(item.GroupIdx);
                writer.Write(item.CastIdx);
            }
        });
    }
}