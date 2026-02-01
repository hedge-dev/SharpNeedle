namespace SharpNeedle.Framework.Ninja.Csd;

public class Font : List<CharacterMapping>, IBinarySerializable
{
    public void Read(BinaryObjectReader reader)
    {
        Clear();
        Capacity = reader.ReadInt32();
        AddRange(reader.ReadObjectArrayOffset<CharacterMapping>(Capacity));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Count);
        writer.WriteObjectCollectionOffset(this);
    }
}