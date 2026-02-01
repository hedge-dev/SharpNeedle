namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

using SharpNeedle.Framework.HedgehogEngine.Mirage.MaterialData;

public class Material : IBinarySerializable
{
    public string? Name { get; set; }
    public string? ShaderName { get; set; }
    public string? TextureName { get; set; }
    public string? DeflectionTextureName { get; set; }
    public TextureAddressMode AddressMode { get; set; }
    public MaterialBlend BlendMode { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringPaddedByte(4);
        ShaderName = reader.ReadStringPaddedByte(4);
        TextureName = reader.ReadStringPaddedByte(4);
        DeflectionTextureName = reader.ReadStringPaddedByte(4);
        AddressMode = (TextureAddressMode)reader.ReadInt32();
        BlendMode = (MaterialBlend)reader.ReadInt32();
        reader.Seek(16, SeekOrigin.Current);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(Name, 4);
        writer.WriteStringPaddedByte(ShaderName, 4);
        writer.WriteStringPaddedByte(TextureName, 4);
        writer.WriteStringPaddedByte(DeflectionTextureName, 4);
        writer.WriteInt32((int)AddressMode);
        writer.WriteInt32((int)BlendMode);
    }
}