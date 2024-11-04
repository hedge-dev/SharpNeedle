namespace SharpNeedle.SonicTeam.DivScene;

public interface IDivDataBlock
{
    void Read(BinaryObjectReader reader, GameType game);
    void Write(BinaryObjectWriter writer, GameType game);
}

public static class DivString
{
    public static string ReadDivString(this BinaryObjectReader reader, int fixedLength = 64)
    {
        // Shift-JIS encoded data
        Span<byte> data = new byte[fixedLength];
        reader.ReadArray(fixedLength, data);

        Encoding shiftJIS = Encoding.GetEncoding(932);
        return shiftJIS.GetString(data).Replace("\0", string.Empty);
    }

    public static void WriteDivString(this BinaryObjectWriter writer, string value, int fixedLength = 64)
    {
        // Shift-JIS encoded data
        Span<byte> data = new byte[fixedLength];

        Encoding shiftJIS = Encoding.GetEncoding(932);
        shiftJIS.GetBytes(value).CopyTo(data);

        writer.WriteArrayFixedLength(data, fixedLength);
    }
}