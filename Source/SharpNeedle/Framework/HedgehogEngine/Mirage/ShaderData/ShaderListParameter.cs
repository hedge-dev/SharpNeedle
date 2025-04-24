namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

public struct ShaderListParameter : IBinarySerializable
{
    public string? Name { get; set; }

    public Vector4 DefaultValue { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
        DefaultValue = new(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, alignment: 4);
        writer.WriteSingle(DefaultValue.X);
        writer.WriteSingle(DefaultValue.Y);
        writer.WriteSingle(DefaultValue.Z);
        writer.WriteSingle(DefaultValue.W);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}
