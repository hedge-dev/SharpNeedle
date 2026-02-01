namespace SharpNeedle.Framework.Ninja.Csd;

public class CastInfoTable : List<(string? Name, int FamilyIdx, int CastIdx)>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        int count = reader.ReadInt32();
        if (count == 0)
        {
            reader.Skip(reader.GetOffsetSize());
            return;
        }

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < count; i++)
            {
                Add(new(reader.ReadStringOffset(), reader.ReadInt32(), reader.ReadInt32()));
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Count);

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
                writer.WriteOffset(() => 
                {
                    writer.WriteString(StringBinaryFormat.NullTerminated, Name);
                    writer.WriteByte(0);
                });
                writer.WriteInt32(FamilyIdx);
                writer.WriteInt32(CastIdx);
            }
        });
    }
}