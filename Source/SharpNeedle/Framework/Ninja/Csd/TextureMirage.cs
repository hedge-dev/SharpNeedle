namespace SharpNeedle.Framework.Ninja.Csd;

public class TextureMirage : IBinarySerializable, ITexture
{
    public string? Name { get; set; }
    public byte[] Data { get; set; } = [];
    public int MemoryDataIndex { get; internal set; }

    public TextureMirage()
    {

    }

    public TextureMirage(string name)
    {
        Name = name;
    }

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        MemoryDataIndex = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        if(Name == null)
        {
            writer.WriteOffsetValue(0);
        }
        else
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        }

        writer.Write(MemoryDataIndex);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}