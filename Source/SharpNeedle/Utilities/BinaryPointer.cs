namespace SharpNeedle.Utilities;

public struct BinaryPointer<T> : IBinarySerializable where T : IBinarySerializable, new()
{
    public T Value;

    public BinaryPointer(T value)
    {
        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read(BinaryObjectReader reader)
    {
        var offset = reader.ReadOffsetValue();
        if (offset == 0)
            return;

        Value = reader.ReadObjectAtOffset<T>(offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(BinaryObjectWriter writer)
    {
        if (Value is null)
            writer.WriteOffsetValue(0);
        else
            writer.WriteObjectOffset(Value);
    }

    public static implicit operator T(BinaryPointer<T> self) => self.Value;
    public static implicit operator BinaryPointer<T>(T value) => new(value);
}

public struct BinaryPointer<T, TContext> : IBinarySerializable<TContext> where T : IBinarySerializable<TContext>, new()
{
    public T Value;

    public BinaryPointer(T value)
    {
        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read(BinaryObjectReader reader, TContext context)
    {
        var offset = reader.ReadOffsetValue();
        if (offset == 0)
            return;

        Value = reader.ReadObjectAtOffset<T, TContext>(offset, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(BinaryObjectWriter writer, TContext context)
    {
        if (Value is null)
            writer.WriteOffsetValue(0);
        else
            writer.WriteObjectOffset(Value, context);
    }

    public static implicit operator T(BinaryPointer<T, TContext> self) => self.Value;
    public static implicit operator BinaryPointer<T, TContext>(T value) => new(value);
}