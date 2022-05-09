namespace SharpNeedle;

public struct BitSet<T> : IEnumerable<bool> where T : IBinaryInteger<T>, IShiftOperators<T, T>
{
    public T Value;
    public int BitCount => Unsafe.SizeOf<T>() * 8;

    public BitSet(T value)
    {
        Value = value;
    }

    public BitSet(params T[] activeBits) : this()
    {
        foreach (var bit in activeBits)
            Set(NumberHelper.Create<int, T>(bit));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(int bit) => (Value & T.One << bit) != T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int bit) => Value |= T.One << bit;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() => Value = T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(int bit) => Value &= ~(T.One << bit);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(int bit, bool value)
    {
        if (value)
            Set(bit);
        else
            Reset(bit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public int PopCount() => NumberHelper.Create<int, T>(T.PopCount(Value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Any() => Value != T.Zero;

    public bool this[int bit]
    {
        get => Test(bit);
        set => Set(bit, value);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        for (int i = 0; i < BitCount; i++)
            yield return Test(i);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator BitSet<T>(T value) => new (value);
    public static implicit operator T(BitSet<T> bits) => bits.Value;

    private static class NumberHelper
    {
        public static TTo Create<TTo, TFrom>(TFrom number) where TTo : IBinaryInteger<TTo>
            where TFrom : IBinaryInteger<TFrom>
            => TTo.Create(number);
    }
}