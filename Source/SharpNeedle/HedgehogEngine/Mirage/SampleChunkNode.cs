namespace SharpNeedle.HedgehogEngine.Mirage;

using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata.Ecma335;

public class SampleChunkNode : IBinarySerializable, IEnumerable<SampleChunkNode>
{
    public const uint RootSignature = 0x133054A;

    private readonly List<SampleChunkNode> _children = [];

    /// <summary>
    /// Offset to the start of the file at which <see cref="Data"/> starts (for internal read/write only)
    /// </summary>
    internal long DataOffset { get; private set; }

    /// <summary>
    /// Size of <see cref="Data"/> in bytes in file (for internal read/write only)
    /// </summary>
    internal long DataSize { get; private set; }


    /// <summary>
    /// Name of the node
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Unsigned 32 bit integer value
    /// </summary>
    public uint Value { get; set; }

    /// <summary>
    /// Signed 32 bit integer value (wraps around <see cref="Value"/>)
    /// </summary>
    public int SignedValue
    {
        get => unchecked((int)Value);
        set => Value = unchecked((uint)value);
    }

    /// <summary>
    /// Float value (wraps around <see cref="Value"/>)
    /// </summary>
    public float FloatValue
    {
        get => BitConverter.UInt32BitsToSingle(Value);
        set => Value = BitConverter.SingleToUInt32Bits(Value);
    }

    /// <summary>
    /// Arbitrary binary data
    /// </summary>
    public IBinarySerializable Data { get; set; }

    /// <summary>
    /// Parent node
    /// </summary>
    public SampleChunkNode Parent { get; private set; }

    /// <summary>
    /// Node children
    /// </summary>
    public ReadOnlyCollection<SampleChunkNode> Children { get; }

    /// <summary>
    /// Number of children inside this node
    /// </summary>
    public int Count => Children.Count;


    public SampleChunkNode()
    {
        Children = new(_children);
    }

    public SampleChunkNode(string name) : this()
    {
        Name = name;
    }

    public SampleChunkNode(string name, uint value) : this(name)
    {
        Value = value;
    }

    public SampleChunkNode(string name, int value) : this(name)
    {
        SignedValue = value;
    }

    public SampleChunkNode(string name, float value) : this(name)
    {
        FloatValue = value;
    }

    public SampleChunkNode(string name, IBinarySerializable data, uint dataVersion) : this(name, dataVersion)
    {
        Data = data;
    }


    /// <summary>
    /// Detaches the node from its parent. Children will be kept.
    /// </summary>
    public void Detach()
    {
        if(Parent != null)
        {
            Parent._children.Remove(this);
            Parent = null;
        }
    }

    /// <summary>
    /// Detaches all children from this node.
    /// </summary>
    public void DetachChildren()
    {
        foreach(SampleChunkNode node in _children.ToArray())
        {
            node.Detach();
        }
    }

    /// <summary>
    /// Inserts the node after the last child node.
    /// </summary>
    /// <param name="node">The node to add.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddChild(SampleChunkNode node)
    {
        if(node.Parent != null)
        {
            throw new InvalidOperationException($"Node \"{node.Name}\" still has a parent! Detach it before adding it to a node.");
        }

        _children.Add(node);
        node.Parent = this;
    }

    /// <summary>
    /// Inserts the node to be a specific child index.
    /// </summary>
    /// <param name="index">Index at which to insert the node.</param>
    /// <param name="node">The node to insert.</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void InsertChild(int index, SampleChunkNode node)
    {
        if(index < 0)
        {
            throw new IndexOutOfRangeException($"Index {index} out of range! Must be a positive value!");
        }
        else if(node.Parent != null)
        {
            throw new InvalidOperationException($"Node \"{node.Name}\" still has a parent! Detach it before adding it to a node.");
        }

        _children.Insert(index, node);
        node.Parent = this;
    }


    /// <summary>
    /// Returns the root node of this tree.
    /// </summary>
    public SampleChunkNode GetRootNode()
    {
        SampleChunkNode root = this;
        while(root.Parent != null)
        {
            root = root.Parent;
        }

        return root;
    }

    /// <summary>
    /// Attempt to find a node by its name. 
    /// <br/> Returns <see langword="null"/> if none was found.
    /// </summary>
    /// <param name="name">Name of the node to look for.</param>
    /// <param name="recursive">Whether to recursively look in child nodes too</param>
    public SampleChunkNode FindNode(string name, bool recursive = true)
    {
        if(string.IsNullOrEmpty(name))
        {
            return null;
        }

        foreach(SampleChunkNode child in this)
        {
            if(child.Name == name)
            {
                return child;
            }

            if(recursive && child.Count > 0)
            {
                SampleChunkNode item = child.FindNode(name, recursive);
                if(item != null)
                {
                    return item;
                }
            }
        }

        return null;
    }


    void IBinarySerializable.Read(BinaryObjectReader reader)
    {
        Read(reader);
    }

    public bool Read(BinaryValueReader reader)
    {
        long startPos = reader.Position;
        uint header = reader.ReadUInt32();

        Flags flags = (Flags)header & Flags.Mask;
        uint size = header & (uint)Flags.SizeMask;

        Value = reader.Read<uint>();
        Name = reader.ReadString(StringBinaryFormat.FixedLength, 8);

        int spaceIdx = Name.IndexOf(' ');
        if(spaceIdx >= 0)
        {
            Name = Name[..spaceIdx];
        }

        if(!flags.HasFlag(Flags.Leaf) || flags.HasFlag(Flags.Root))
        {
            SampleChunkNode node;
            do
            {
                node = new();
                AddChild(node);
            }
            while(node.Read(reader));
        }

        long end = startPos + size;
        if(end > reader.Position)
        {
            DataOffset = reader.Position;
            DataSize = end - DataOffset;
            reader.Skip((int)DataSize);
        }

        return !flags.HasFlag(Flags.LastChild);
    }

    public void Write(BinaryObjectWriter writer)
    {
        Write(writer, false);
    }

    private void Write(BinaryObjectWriter writer, bool lastNode)
    {
        long start = writer.Position;
        SeekToken flagsToken = writer.At();
        writer.Write(0u);

        Flags flags = default;
        if(Parent == null)
        {
            flags = Flags.Root;
            writer.Write(RootSignature);
            writer.Write(0ul);
            writer.PushOffsetOrigin();
        }
        else
        {
            if(lastNode)
            {
                flags = Flags.LastChild;
            }

            writer.Write(Value);
            writer.WriteLittle(BinaryHelper.MakeSignature<ulong>(Name, 0x20));
        }

        if(Count > 0)
        {
            for(int i = 0; i < Count; i++)
            {
                Children[i].Write(writer, i == Count - 1);
            }
        }
        else
        {
            flags |= Flags.Leaf;
        }

        if(Data != null)
        {
            DataOffset = writer.Position;
            writer.WriteObject(Data);
            // data has to be flushed to be self-contained
            writer.Flush();
            DataSize = writer.Position - DataOffset;
        }
        else
        {
            DataOffset = 0;
            DataSize = 0;
        }

        if(Parent == null)
        {
            // Collect offsets and write them
            // instead of a name, the root node has an offset to the offset table, and an offset count
            long[] offsets = writer.OffsetHandler.OffsetPositions.ToArray();

            writer.PopOffsetOrigin();
            using SeekToken token = writer.At(start + 8, SeekOrigin.Begin);

            writer.WriteOffset(() =>
            {
                // Offsets are relative to beginning of node contents
                long nodeContentOffset = start + 0x10;
                foreach(long offset in offsets)
                {
                    writer.Write((uint)(offset - nodeContentOffset));
                }
            });

            writer.Write(offsets.Length);
        }
        else if(lastNode && Parent.Parent == null)
        {
            // Right before offset table is written, align by 0x10
            int remainingAlignment = 0x10 - (int)(writer.Position % 0x10);
            if(remainingAlignment < 0x10)
            {
                if(DataOffset == 0)
                {
                    DataOffset = writer.Position;
                }

                DataSize += remainingAlignment;
                writer.WriteNulls(remainingAlignment);
            }
        }

        using SeekToken endToken = writer.At();
        flagsToken.Dispose();
        writer.Write((uint)((long)endToken - start) | (uint)flags);
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
