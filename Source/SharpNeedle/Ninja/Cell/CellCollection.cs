namespace SharpNeedle.Ninja.Cell;

public class CellCollection<T> : Dictionary<string, T>, IBinarySerializable where T : IBinarySerializable, new()
{
    public NameIndexPair[] IndexTable { get; private set; }

    public void Read(BinaryObjectReader reader)
    {
        Clear();
        var count = reader.Read<int>();
        if (count == 0)
        {
            reader.Skip(reader.GetOffsetSize() * 2);
            return;
        }

        var items = reader.ReadObjectArrayOffset<T>(count);
        IndexTable = reader.ReadObjectArrayOffset<NameIndexPair>(count);

        foreach (var pair in IndexTable)
            Add(pair.Name, items[pair.Index]);
    }

    public void Write(BinaryObjectWriter writer)
    {
        if (Count == 0)
        {
            writer.Write(0);
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            return;
        }

        var values = this.ToArray();
        IndexTable = new NameIndexPair[values.Length];
        writer.Write(values.Length);
        writer.WriteOffset(() =>
        {
            foreach (var pair in values)
                writer.WriteObject(pair.Value);
        });

        writer.WriteOffset(() =>
        {
            for (int i = 0; i < values.Length; i++)
            {
                IndexTable[i] = new NameIndexPair(values[i].Key, i);
                writer.WriteObject(IndexTable[i]);
            }
        });
    }

    public T Find(int idx)
    {
        foreach (var table in IndexTable)
        {
            if (table.Index == idx)
                return this[table.Name];
        }

        return default;
    }

    public T this[int idx] => Find(idx);

    public struct NameIndexPair : IBinarySerializable
    {
        public string Name;
        public int Index;

        public NameIndexPair(string name, int idx)
        {
            Name = name;
            Index = idx;
        }

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
            Index = reader.Read<int>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(ref Index);
        }
    }
}