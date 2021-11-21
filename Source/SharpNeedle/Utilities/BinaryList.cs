namespace SharpNeedle.Utilities;
public struct BinaryList<T> : IBinarySerializable, IList<T> where T : IBinarySerializable, new()
{
    public List<T> Items { get; set; }
    public int Count => Items.Count;
    public bool IsReadOnly => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read(BinaryObjectReader reader)
    {
        var count = reader.ReadOffsetValue();
        Items = new List<T>((int)count);

        // Use token because accessing `this` with lambda on structs is a nightmare
        using var token = reader.ReadOffset();
        for (long i = 0; i < count; i++)
        {
            var obj = reader.ReadObject<T>();
            Items.Add(obj);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteOffsetValue(Items.Count);
        writer.WriteOffset(writer.DefaultAlignment, null, Items, (_, items) =>
        {
            foreach (var item in (List<T>)items)
                writer.WriteObject(item);
        });
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        Items.Add(item);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public bool Contains(T item)
    {
        return Items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return Items.Remove(item);
    }
    
    public int IndexOf(T item)
    {
        return Items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        Items.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Items.RemoveAt(index);
    }

    public T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }
}