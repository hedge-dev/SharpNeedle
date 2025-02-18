namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader.Variable;

public struct ConstantBufferField : IBinarySerializable
{
    private string? _name;

    /// <summary>
    /// This is rarely "2", but no idea what it does
    /// </summary>
    public int Unknown1 { get; set; }

    public int ConstantBufferIndex { get; set; }

    public int Offset { get; set; }

    public int Size { get; set; }

    public string Name
    {
        readonly get => _name ?? string.Empty;
        set => _name = value;
    }


    public void Read(BinaryObjectReader reader)
    {
        Unknown1 = reader.ReadInt32();
        ConstantBufferIndex = reader.ReadInt32();
        Offset = reader.ReadInt32();
        Size = reader.ReadInt32();
        Name = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Unknown1);
        writer.WriteInt32(ConstantBufferIndex);
        writer.WriteInt32(Offset);
        writer.WriteInt32(Size);
        writer.WriteString(StringBinaryFormat.NullTerminated, Name);
        writer.Align(4);
    }


    public override string ToString()
    {
        return Name;
    }
}
