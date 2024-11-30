namespace SharpNeedle.Utilities;

public struct BinaryPrimitive<T> : IBinarySerializable where T : unmanaged
{
    public T Value;

    public BinaryPrimitive(T value)
    {
        Value = value;
    }

    public void Read(BinaryObjectReader reader)
    {
        reader.Read(out Value);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(ref Value);
    }

    public static implicit operator T(BinaryPrimitive<T> self)
    {
        return self.Value;
    }

    public static implicit operator BinaryPrimitive<T>(T value)
    {
        return new(value);
    }
}