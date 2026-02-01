namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

public struct ShaderListParameter : IBinarySerializable
{
    public string? Name { get; set; }

    public Vector4 DefaultValue { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
        DefaultValue = reader.ReadVector4();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, alignment: 4);
        writer.WriteVector4(DefaultValue);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}
