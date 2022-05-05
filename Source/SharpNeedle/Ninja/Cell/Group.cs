namespace SharpNeedle.Ninja.Cell;

public class Group : IBinarySerializable
{
    public List<Cast> Casts { get; set; }
    public uint Field08 { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Casts = reader.ReadObject<BinaryList<BinaryPointer<Cast>>>().Unwind();
        Field08 = reader.Read<uint>();
        var tree = reader.ReadArrayOffset<TreeDescriptorNode>(Casts.Count);
    }

    public void Write(BinaryObjectWriter writer)
    {
        throw new NotImplementedException();
    }

    struct TreeDescriptorNode
    {
        public int ChildIndex;
        public int SiblingIndex;
    }
}