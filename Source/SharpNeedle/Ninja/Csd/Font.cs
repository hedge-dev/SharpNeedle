namespace SharpNeedle.Ninja.Csd;

public class Font : List<CharacterMapping>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        Clear();
        Capacity = reader.Read<int>();
        AddRange(reader.ReadArrayOffset<CharacterMapping>(Capacity));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Count);
        writer.WriteCollectionOffset(this);
    }
}