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
        Field00 = reader.Read<uint>();
        Name = reader.ReadStringOffset();
        Field08 = reader.Read<ushort>();
        Field0A = reader.Read<ushort>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Field08);
        writer.Write(Field0A);
        writer.Write(Field0C);
        writer.Write(Field10);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}