namespace SharpNeedle.Structs;

public struct BitSet<T> : IEnumerable<bool> where T : INumberBase<T>, IBinaryInteger<T>
{
    public T Value;
    public readonly int BitCount => Unsafe.SizeOf<T>() * 8;

    public BitSet(T value)
    {
        Value = value;
    }

    public BitSet(params T[] activeBits) : this()
    {
        foreach(T bit in activeBits)
        {
            Set(NumberHelper.Create<int, T>(bit));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T GetField(Range range)
    {
        unchecked
        {
            int start = range.Start.IsFromEnd ? BitCount - range.Start.Value : range.Start.Value;
            int end = range.End.IsFromEnd ? BitCount - range.End.Value : range.End.Value;
            return Value >> start & (T.One << end - start) - T.One;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void SetField(Range range, int value)
    {
        unchecked
        {
            int start = range.Start.IsFromEnd ? BitCount - range.Start.Value : range.Start.Value;
            int end = range.End.IsFromEnd ? BitCount - range.End.Value : range.End.Value;
            T mask = (T.One << end - start) - T.One << start;
            Value = Value & mask | T.CreateChecked(value & (1 << end - start) - 1);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Test(int bit)
    {
        return (Value & T.One << bit) != T.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int bit)
    {
        Value |= T.One << bit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        Value = T.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset(int bit)
    {
        Value &= ~(T.One << bit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Set(int bit, bool value)
    {
        if(value)
        {
            Set(bit);
        }
        else
        {
            Reset(bit);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int PopCount()
    {
        return NumberHelper.Create<int, T>(T.PopCount(Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Any()
    {
        return Value != T.Zero;
    }

    public bool this[int bit]
    {
        get => Test(bit);
        set => Set(bit, value);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        for(int i = 0; i < BitCount; i++)
        {
            yield return Test(i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override readonly string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator BitSet<T>(T value)
    {
        return new(value);
    }

    public static implicit operator T(BitSet<T> bits)
    {
        return bits.Value;
    }

    private static class NumberHelper
    {
        public static TTo Create<TTo, TFrom>(TFrom number) where TTo : IBinaryInteger<TTo>
            where TFrom : IBinaryInteger<TFrom>
        {
            return TTo.CreateChecked(number);
        }
    }
}