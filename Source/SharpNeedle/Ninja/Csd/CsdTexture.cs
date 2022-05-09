namespace SharpNeedle.Ninja.Csd;

public class CsdTexture : IBinarySerializable
{
    public string Name { get; set; }
    public byte[] Data { get; set; }
    public int MemoryDataIndex { get; internal set; }
    
    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        MemoryDataIndex = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        if (Name == null)
            writer.WriteOffsetValue(0);
        else
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        
        writer.Write(MemoryDataIndex);
    }
}