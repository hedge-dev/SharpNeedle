namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

public struct ShaderListTexture : IBinarySerializable
{
    public string? Name { get; set; }

    public uint Unknown { get; set; }


    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
        Unknown = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, alignment: 4);
        writer.WriteUInt32(Unknown);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}
