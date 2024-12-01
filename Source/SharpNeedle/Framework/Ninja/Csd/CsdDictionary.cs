namespace SharpNeedle.Framework.Ninja.Csd;

public class CsdDictionary<T> : IBinarySerializable, IDictionary<string, T> where T : IBinarySerializable, new()
{
    internal static readonly NameIndexPair.Comparer NameIndexPairComparer = new();

    private List<T> Items { get; } = [];
    private List<NameIndexPair> NameTable { get; } = [];
    public int Count => Items.Count;
    public ICollection<string> Keys => NameTable.Select(x => x.Name).ToList();
    public ICollection<T> Values => Items;
    public bool IsReadOnly => false;

    public void Insert(string key, T value, int index)
    {
        int nameIdx = NameTable.BinarySearch(new NameIndexPair(key, 0), NameIndexPairComparer);
        if(nameIdx >= 0)
        {
            throw new Exception($"Key {key} already exists");
        }

        // Fix indices then insert
        Span<NameIndexPair> table = CollectionsMarshal.AsSpan(NameTable);
        for(int i = 0; i < table.Length; i++)
        {
            if(table[i].Index >= index)
            {
                table[i].Index++;
            }
        }

        Items.Insert(index, value);
        NameTable.Insert(~nameIdx, new NameIndexPair(key, index));
    }

    public void Add(KeyValuePair<string, T> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Items.Clear();
        NameTable.Clear();
    }

    public bool Contains(KeyValuePair<string, T> item)
    {
        return ContainsKey(item.Key);
    }

    public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
    {
        if(Count > array.Length - arrayIndex)
        {
            throw new ArgumentException("Array is not large enough");
        }

        int i = 0;
        foreach(KeyValuePair<string, T> pair in this)
        {
            array[i++ + arrayIndex] = pair;
        }
    }

    public bool Remove(KeyValuePair<string, T> item)
    {
        return Remove(item.Key);
    }

    public void Read(BinaryObjectReader reader)
    {
        int count = reader.Read<int>();
        if(count == 0)
        {
            reader.Skip(reader.GetOffsetSize() * 2);
            return;
        }

        Items.AddRange(reader.ReadObjectArrayOffset<T>(count));
        NameTable.AddRange(reader.ReadObjectArrayOffset<NameIndexPair>(count));
    }

    public void Write(BinaryObjectWriter writer)
    {
        if(Items?.Count is null or 0)
        {
            writer.Write(0);
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            return;
        }

        writer.Write(NameTable.Count);
        writer.WriteObjectCollectionOffset(Items);
        writer.WriteObjectCollectionOffset(NameTable);
    }

    public int Find(string key)
    {
        int result = NameTable.BinarySearch(new NameIndexPair(key, 0), NameIndexPairComparer);
        if(result < 0)
        {
            return -1;
        }

        return NameTable[result].Index;
    }

    public void Add(string key, T value)
    {
        if(Items.Count == 0)
        {
            Items.Add(value);
            NameTable.Add(new(key, 0));
            return;
        }

        int index = NameTable.BinarySearch(new NameIndexPair(key, 0), NameIndexPairComparer);
        if(index >= 0)
        {
            throw new ArgumentException($"Key {key} already exists");
        }

        Items.Add(value);
        NameTable.Insert(~index, new NameIndexPair(key, Items.Count - 1));
    }

    public bool ContainsKey(string key)
    {
        return Find(key) >= 0;
    }

    public bool Remove(string key)
    {
        int index = NameTable.BinarySearch(new NameIndexPair(key, 0), NameIndexPairComparer);
        if(index < 0)
        {
            return false;
        }

        Items.RemoveAt(NameTable[index].Index);
        PostRemove(NameTable[index].Index);
        NameTable.RemoveAt(index);
        return true;
    }

    public bool RemoveAt(int idx)
    {
        int index = NameTable.FindIndex(x => x.Index == idx);
        if(index == -1)
        {
            return false;
        }

        NameTable.RemoveAt(index);
        Items.RemoveAt(idx);
        PostRemove(idx);
        return true;
    }

    private void PostRemove(int idx)
    {
        Span<NameIndexPair> table = CollectionsMarshal.AsSpan(NameTable);
        for(int i = 0; i < table.Length; i++)
        {
            if(table[i].Index > idx)
            {
                table[i].Index--;
            }
        }
    }

    public bool TryGetValue(string key, out T value)
    {
        int index = NameTable.BinarySearch(new NameIndexPair(key, 0), NameIndexPairComparer);
        if(index < 0)
        {
            value = default;
            return false;
        }

        value = Items[NameTable[index].Index];
        return true;
    }

    public T this[string key]
    {
        get => Items[Find(key)];
        set => Items[Find(key)] = value;
    }

    public T this[int idx]
    {
        get => Items[idx];
        set => Items[idx] = value;
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        for(int i = 0; i < Items.Count; i++)
        {
            for(int j = 0; j < NameTable.Count; j++)
            {
                if(NameTable[j].Index == i)
                {
                    yield return new KeyValuePair<string, T>(NameTable[j].Name, Items[i]);
                    break;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

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

        internal class Comparer : IComparer<NameIndexPair>
        {
            public int Compare(NameIndexPair x, NameIndexPair y)
            {
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }
    }
}