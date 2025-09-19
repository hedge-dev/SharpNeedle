namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

using SharpNeedle.Framework.HedgehogEngine.Mirage.MaterialData;

public class Material : IBinarySerializable
{
    public string? Name;
    public string? ShaderName;
    public string? TextureName;
    public string? DeflectionTextureName;
    public MaterialAddress AddressMode; 
    public MaterialBlendMode BlendMode;
    
    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringPaddedByte();
        ShaderName = reader.ReadStringPaddedByte();
        TextureName = reader.ReadStringPaddedByte();
        DeflectionTextureName = reader.ReadStringPaddedByte();
        AddressMode = (MaterialAddress)reader.ReadInt32();
        BlendMode = (MaterialBlendMode)reader.ReadInt32();
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