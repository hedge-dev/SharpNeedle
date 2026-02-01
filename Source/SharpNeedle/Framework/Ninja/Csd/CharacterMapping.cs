namespace SharpNeedle.Framework.Ninja.Csd;

public struct CharacterMapping : IBinarySerializable
{
    public int SourceIndex;
    public int DestinationIndex;

    public void Read(BinaryObjectReader reader)
    {
        SourceIndex = reader.ReadInt32();
        DestinationIndex = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(SourceIndex);
        writer.WriteInt32(DestinationIndex);
    }
}