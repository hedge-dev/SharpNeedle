namespace SharpNeedle.Utilities;

public static class BinaryHelper
{
    public static unsafe TSize MakeSignature<TSize>(string sig, byte placeholder = 0) where TSize : unmanaged
    {
        Span<byte> result = stackalloc byte[Unsafe.SizeOf<TSize>()];
        result.Fill(placeholder);

        for (int i = 0; i < Math.Min(sig.Length, Unsafe.SizeOf<TSize>()); i++)
            result[i] = (byte)sig[i];

        return Unsafe.As<byte, TSize>(ref result[0]);
    }

    public static string ReadStringOffset(this BinaryObjectReader reader, StringBinaryFormat format = StringBinaryFormat.NullTerminated, int fixedLength = -1)
    {
        var offset = reader.ReadOffsetValue();
        if (offset == 0)
            return null;

        using var token = reader.AtOffset(offset);
        return reader.ReadString(format, fixedLength);
    }
}