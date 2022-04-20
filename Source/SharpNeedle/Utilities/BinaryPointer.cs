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