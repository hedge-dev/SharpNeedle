namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader.Variable;

public enum UnorderedAccessViewType
{
    RWTexture2D = 3,
    RWBuffer = 10
}

public struct UnorderedAccessView : IBinarySerializable
{
    private string? _name;

    public UnorderedAccessViewType Type { get; set; }

    public int ID { get; set; }

    public string Name
    {
        readonly get => _name ?? string.Empty;
        set => _name = value;
    }

    public void Read(BinaryObjectReader reader)
    {
        Type = (UnorderedAccessViewType)reader.ReadInt32();
        ID = reader.ReadInt32();
        Name = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)Type);
        writer.WriteInt32(ID);
        writer.WriteString(StringBinaryFormat.NullTerminated, Name);
        writer.Align(4);
    }

    public override string ToString()
    {
        return Name;
    }
}
