namespace SharpNeedle.HedgehogEngine.Mirage;
using System.IO;

public class SampleChunkNode : IBinarySerializable, IEnumerable<SampleChunkNode>
{
    public const uint RootSignature = 0x133054A;

    public string Name { get; set; }
    public uint Value { get; set; }
    public IBinarySerializable Data { get; set; }
    public SampleChunkNode Parent { get; internal set; }
    public List<SampleChunkNode> Children { get; set; } = new();
    public int Count => Children.Count;

    internal long DataOffset { get; set; }
    internal uint Size { get; set; }

    public SampleChunkNode()
    {

    }

    public SampleChunkNode(SampleChunkNode parent)
    {
        Parent = parent;
    }

    void IBinarySerializable.Read(BinaryObjectReader reader)
    {
        Read(reader);
    }

    public bool Read(BinaryValueReader reader)
    {
        var flags = reader.Read<Flags>() & Flags.Mask;
        Size = (uint)(flags & Flags.SizeMask);
        Value = reader.Read<uint>();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 8);
        var spaceIdx = Name.IndexOf(' ');
        if (spaceIdx >= 0)
            Name = Name[..spaceIdx];

        DataOffset = reader.Position;

        if (!flags.HasFlag(Flags.Leaf) || flags.HasFlag(Flags.Root))
        {
            while (true)
            {
                var node = new SampleChunkNode(this);
                Children.Add(node);
                if (!node.Read(reader))
                    break;
            }
        }

        return !flags.HasFlag(Flags.LastChild);
    }

    public void Write(BinaryObjectWriter writer)
    {
        Write(writer, false);
    }

    private void Write(BinaryObjectWriter writer, bool lastNode)
    {
        var start = writer.Position;
        var flagsToken = writer.At();
        writer.Write(0u);

        var flags = lastNode ? Flags.LastChild : Flags.Leaf;
        if (Parent == null)
        {
            flags = Flags.Root;
            writer.Write(RootSignature);
            writer.Write(0ul);
            writer.PushOffsetOrigin();
        }
        else
        {
            writer.Write(Value);
            writer.WriteLittle(BinaryHelper.MakeSignature<ulong>(Name, 0x20));
        }

        DataOffset = writer.Position;
        if (Count > 0)
        {
            for (int i = 0; i < Count; i++)
                Children[i].Write(writer, i == Count - 1);
        }
        else
            flags |= Flags.Leaf;

        if (Data != null)
            writer.WriteObject(Data);

        if (Parent == null)
        {
            // Collect offsets and write them
            writer.Flush();

            using var token = new SeekToken(writer.GetBaseStream(), DataOffset - 8, SeekOrigin.Begin);
            var offsets = writer.OffsetHandler.OffsetPositions.ToArray();

            writer.PopOffsetOrigin();

            writer.WriteOffset(() =>
            {
                // Offsets are relative to beginning of data
                foreach (var offset in offsets)
                    writer.Write((uint)(offset - DataOffset));
            });

            writer.Write(offsets.Length);
        }

        using var endToken = writer.At();
        flagsToken.Dispose();
        writer.Write((uint)((long)endToken - start) | (uint)flags);
    }

    public SampleChunkNode Find(string name, bool recursive = true)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        foreach (var child in this)
        {
            if (child.Name == name)
                return child;

            if (recursive && child.Count > 0)
            {
                var item = child.Find(name, recursive);
                if (item != null)
                    return item;
            }
        }

        return null;
    }

    public SampleChunkNode Add(string name)
    {
        return Add(name, 0, null);
    }

    public SampleChunkNode Add(string name, IBinarySerializable data)
    {
        return Add(name, 0, data);
    }

    public SampleChunkNode Add(string name, uint value, IBinarySerializable data)
    {
        var node = new SampleChunkNode(this) { Name = name, Value = value, Data = data};
        Children.Add(node);
        return node;
    }

    public IEnumerator<SampleChunkNode> GetEnumerator()
    {
        return ((IEnumerable<SampleChunkNode>)Children).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return Name;
    }

    [Flags]
    public enum Flags : uint
    {
        None = 0,
        Leaf = 0x20000000,
        LastChild = 0x40000000,
        Root = 0x80000000,

        SizeMask = 0x1FFFFFFF,
        Mask = 0xE0000000U
    }
}
