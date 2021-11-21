namespace SharpNeedle.HedgehogEngine.Mirage;

public class TextureUnit : IBinarySerializable
{
    public string Name { get; set; }
    public byte Index { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        Index = reader.ReadByte();
        reader.Align(4);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Index);
        writer.Align(4);
    }
}