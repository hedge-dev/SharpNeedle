namespace SharpNeedle;
using Color8 = Color<byte>;

[StructLayout(LayoutKind.Sequential)]
public struct Color<T> where T : struct, INumber<T>
{
    public T R;
    public T G;
    public T B;
    public T A;

    public Color(T r, T g, T b, T a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(ReadOnlySpan<T> values)
    {
        if (values.Length < 4)
            throw new IndexOutOfRangeException();

        this = Unsafe.ReadUnaligned<Color<T>>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
    }

    public unsafe Span<T> AsSpan()
    {
        return new Span<T>(Unsafe.AsPointer(ref this), 4);
    }

    public static implicit operator Vector<T>(Color<T> value)
    {
        return new Vector<T>(value.AsSpan());
    }

    public override string ToString()
    {
        return $"{{ R:{R}, G:{G}, B:{B}, A:{A} }}";
    }
}

public static class ColorExtensions
{
    public static Vector4 AsVector(this Color<float> color)
        => new (color.AsSpan());

    public static unsafe Color<float> AsColor(this Vector4 color)
        => new (new ReadOnlySpan<float>(Unsafe.AsPointer(ref color), 4));
}