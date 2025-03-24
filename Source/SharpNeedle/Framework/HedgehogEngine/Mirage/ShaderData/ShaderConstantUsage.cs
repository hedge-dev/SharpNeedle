namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

public class ShaderConstantUsage : IBinarySerializable
{
    public string? Name { get; set; }
    public byte Index { get; set; }
    public byte Size { get; set; } = 1;
    public ShaderConstantType Type { get; internal set; } = ShaderConstantType.Unknown;

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        Index = reader.Read<byte>();
        Size = reader.Read<byte>();

        reader.Align(reader.GetOffsetSize());
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Index);
        writer.Write(Size);

        writer.Align(writer.GetOffsetSize());
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}