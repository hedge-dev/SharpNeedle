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
        Name = reader.ReadStringPaddedByte();
        ShaderName = reader.ReadStringPaddedByte();
        TextureName = reader.ReadStringPaddedByte();
        DeflectionTextureName = reader.ReadStringPaddedByte();
        AddressMode = (TextureAddressMode)reader.ReadInt32();
        BlendMode = (MaterialBlend)reader.ReadInt32();
        reader.Seek(16, SeekOrigin.Current);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(Name);
        writer.WriteStringPaddedByte(ShaderName);
        writer.WriteStringPaddedByte(TextureName);
        writer.WriteStringPaddedByte(DeflectionTextureName);
        writer.Write(AddressMode);
        writer.Write(BlendMode);
    }
}