namespace SharpNeedle.Frontiers.DVScene;

public static class DVString
{
    public static string ReadDVString(this BinaryObjectReader reader, int fixedLength = 64)
    {
        // Shift-JIS encoded data
        Span<byte> data = fixedLength <= 256 ? stackalloc byte[fixedLength] : new byte[fixedLength];
        reader.ReadArray(fixedLength, data);

        Encoding shiftJIS = Encoding.GetEncoding(932);
        return shiftJIS.GetString(data).Replace("\0", string.Empty);
    }

    public static void WriteDVString(this BinaryObjectWriter writer, string value, int fixedLength = 64)
    {
        // Shift-JIS encoded data
        Span<byte> data = fixedLength <= 256 ? stackalloc byte[fixedLength] : new byte[fixedLength];

        Encoding shiftJIS = Encoding.GetEncoding(932);
        shiftJIS.GetBytes(value).CopyTo(data);

        writer.WriteArrayFixedLength(data, fixedLength);
    }
}