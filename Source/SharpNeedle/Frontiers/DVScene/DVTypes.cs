namespace SharpNeedle.Frontiers.DVScene;

public struct DVGuid
{
    public uint G0;
    public uint G4;
    public uint G8;
    public uint GC;
}

public static class DVString
{
    public static string ReadDVString(this BinaryObjectReader reader, int fixedLength = 64)
    {
        // Shift-JIS encoded data
        byte[] data = new byte[fixedLength];
        reader.ReadArray<byte>(fixedLength, data);

        return Encoding.GetEncoding(932).GetString(data).Replace("\0", string.Empty);
    }

    public static void WriteDVString(this BinaryObjectWriter writer, string value, int fixedLength = 64)
    {
        // Shift-JIS encoded data
        byte[] data = new byte[fixedLength];
        Encoding.GetEncoding(932).GetBytes(value).CopyTo(data, 0);

        writer.WriteArrayFixedLength(data, fixedLength);
    }
}