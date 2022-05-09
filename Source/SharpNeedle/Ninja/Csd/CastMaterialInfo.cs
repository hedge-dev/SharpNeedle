namespace SharpNeedle.Ninja.Csd;

public class CastMaterialInfo : IBinarySerializable
{
    public int[] ImageIndices { get; private set; } = new int[32];

    public void Read(BinaryObjectReader reader)
    {
        ImageIndices = reader.ReadArray<int>(32);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteArray(ImageIndices);
    }
}