namespace SharpNeedle.Framework.Ninja.Csd;

public class TextureNN : ITexture, IBinarySerializable
{
    public uint Field00 { get; set; }
    public string? Name { get; set; }
    public ushort Field08 { get; set; } = 5; // Always 5
    public ushort Field0A { get; set; } = 1; // Always 1
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }

    public TextureNN()
    {

    }

    public TextureNN(string name)
    {
        Name = name;
    }

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.ReadUInt32();
        Name = reader.ReadStringOffset();
        Field08 = reader.ReadUInt16();
        Field0A = reader.ReadUInt16();
        Field0C = reader.ReadUInt32();
        Field10 = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Field00);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteUInt16(Field08);
        writer.WriteUInt16(Field0A);
        writer.WriteUInt32(Field0C);
        writer.WriteUInt32(Field10);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}