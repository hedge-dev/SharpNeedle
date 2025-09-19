namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;
public class Material : IBinarySerializable
{
    public string materialName;
    public string shaderName;
    public string textureName;
    public string deflectionTextureName;
    public int addressMode; 
    public int blendMode;
    
    public void Read(BinaryObjectReader reader)
    {
        materialName = reader.ReadStringPaddedByte();
        shaderName = reader.ReadStringPaddedByte();
        textureName = reader.ReadStringPaddedByte();
        deflectionTextureName = reader.ReadStringPaddedByte();
        addressMode = reader.ReadInt32();
        blendMode = reader.ReadInt32();
        reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(materialName);
        writer.WriteStringPaddedByte(shaderName);
        writer.WriteStringPaddedByte(textureName);
        writer.WriteStringPaddedByte(deflectionTextureName);
        writer.Write(addressMode);
        writer.Write(blendMode);
    }
}