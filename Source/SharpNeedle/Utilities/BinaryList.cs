namespace SharpNeedle.Utilities;

public struct BinaryList<T> : IBinarySerializable, IList<T> where T : IBinarySerializable, new()
{
    public List<T> Items { get; set; }
    public readonly int Count => Items.Count;
    public readonly bool IsReadOnly => false;

    public BinaryList(List<T> items)
    {
        Items = items;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read(BinaryObjectReader reader)
    {
        long count = reader.ReadOffsetValue();

        // Use token because accessing `this` with lambda on structs is a nightmare
        long offset = reader.ReadOffsetValue();
        if (offset == 0)
        {
            Items = [];
            return;
        }

        Items = new List<T>((int)count);
        using SeekToken token = reader.AtOffset(offset);
        for (long i = 0; i < count; i++)
        {
            T obj = reader.ReadObject<T>();
            Items.Add(obj);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Write(BinaryObjectWriter writer)
    {
        if (Items == null)
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            return;
        }

        writer.WriteOffsetValue(Items.Count);
        writer.WriteOffset(writer.DefaultAlignment, null, Items, (_, items) =>
        {
            foreach (T item in (List<T>)items)
            {
                writer.WriteObject(item);
            }
        });
    }

    public readonly IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public readonly void Add(T item)
    {
        Items.Add(item);
    }

    public readonly void Clear()
    {
        Items.Clear();
    }

    public readonly bool Contains(T item)
    {
        return Items.Contains(item);
    }

    public readonly void CopyTo(T[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    public readonly bool Remove(T item)
    {
        return Items.Remove(item);
    }

    public readonly int IndexOf(T item)
    {
        return Items.IndexOf(item);
    }

    public readonly void Insert(int index, T item)
    {
        Items.Insert(index, item);
    }

    public readonly void RemoveAt(int index)
    {
        Items.RemoveAt(index);
    }

    public readonly T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public static implicit operator BinaryList<T>(List<T> items)
    {
        return new(items);
    }

    public static implicit operator List<T>(BinaryList<T> self)
    {
        return self.Items;
    }
}

public struct BinaryList<T, TContext> : IBinarySerializable<TContext>, IList<T> where T : IBinarySerializable<TContext>, new()
{
    public List<T> Items { get; set; }
    public readonly int Count => Items.Count;
    public readonly bool IsReadOnly => false;

    public BinaryList(List<T> items)
    {
        Items = items;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read(BinaryObjectReader reader, TContext context)
    {
        long count = reader.ReadOffsetValue();

        // Use token because accessing `this` with lambda on structs is a nightmare
        long offset = reader.ReadOffsetValue();
        if (offset == 0)
        {
            return;
        }

        Items = new List<T>((int)count);
        using SeekToken token = reader.AtOffset(offset);
        for (long i = 0; i < count; i++)
        {
            T obj = reader.ReadObject<T, TContext>(context);
            Items.Add(obj);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Write(BinaryObjectWriter writer, TContext context)
    {
        if (Items == null)
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            return;
        }

        writer.WriteOffsetValue(Items.Count);
        writer.WriteOffset(writer.DefaultAlignment, null, Items, (_, items) =>
        {
            foreach (T item in (List<T>)items)
            {
                writer.WriteObject(item, context);
            }
        });
    }

    public readonly IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public readonly void Add(T item)
    {
        Items.Add(item);
    }

    public readonly void Clear()
    {
        Items.Clear();
    }

    public readonly bool Contains(T item)
    {
        return Items.Contains(item);
    }

    public readonly void CopyTo(T[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    public readonly bool Remove(T item)
    {
        return Items.Remove(item);
    }

    public readonly int IndexOf(T item)
    {
        return Items.IndexOf(item);
    }

    public readonly void Insert(int index, T item)
    {
        Items.Insert(index, item);
    }

    public readonly void RemoveAt(int index)
    {
        Items.RemoveAt(index);
    }

    public readonly T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public static implicit operator BinaryList<T, TContext>(List<T> items)
    {
        return new(items);
    }

    public static implicit operator List<T>(BinaryList<T, TContext> self)
    {
        return self.Items;
    }
}