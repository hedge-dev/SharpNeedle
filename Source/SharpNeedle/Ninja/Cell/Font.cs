namespace SharpNeedle.Ninja.Cell;

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

public struct CharacterMapping
{
    public int SourceIndex;
    public int DestinationIndex;
}