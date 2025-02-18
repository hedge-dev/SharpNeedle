namespace SharpNeedle.Framework.Ninja.Csd;

public class CastInfoTable : List<(string? Name, int FamilyIdx, int CastIdx)>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        int count = reader.Read<int>();
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

        Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        writer.WriteOffset(() =>
        {
            foreach ((string? Name, int FamilyIdx, int CastIdx) in this)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
                writer.Write(FamilyIdx);
                writer.Write(CastIdx);
            }
        });
    }
}