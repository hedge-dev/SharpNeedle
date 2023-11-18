using SharpNeedle.Ninja.Csd;
using System.Linq;

namespace SharpNeedle.SurfRide.Draw;

public class CastNode : IBinarySerializable<ChunkBinaryOptions>, IList<CastNode>
{
    private int mPriority;
    public int Count => Children.Count;
    public bool IsReadOnly => false;

    public string Name { get; set; }
    public int ID { get; set; }
    public CastNodeAttribute Flags { get; set; }
    public short ChildIndex { get; set; }
    public short SiblingIndex { get; set; }
    public ICastData Data { get; set; }
    public UserData UserData { get; set; }
    public Cell Cell { get; set; }
    public CastNode Parent { get; set; }
    public Layer Layer { get; internal set; }
    public List<CastNode> Children { get; set; } = new();

    public int Priority
    {
        get => mPriority;
        set
        {
            var oldPriority = mPriority;
            mPriority = value;

            if (oldPriority != mPriority)
                Layer?.NotifyPriorityChanged(this);
        }
    }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Layer = (Layer)options.Data;

        Name = reader.ReadStringOffset();
        ID = reader.Read<int>();
        Flags = reader.Read<CastNodeAttribute>();
        switch ((int)Flags & 0xF)
        {
            case 1:
                Data = reader.ReadObjectOffset<ImageCastData, ChunkBinaryOptions>(options);
                break;
            case 2:
                Data = reader.ReadObjectOffset<SliceCastData, ChunkBinaryOptions>(options);
                break;
            case 3:
                Data = reader.ReadObjectOffset<ReferenceCastData, ChunkBinaryOptions>(options);
                break;
            default:
                reader.ReadOffsetValue(); // 0 == NullCast
                break;
        }
        
        ChildIndex = reader.Read<short>();
        SiblingIndex = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        Layer = (Layer)options.Data;

        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(ID);
        writer.Write(Flags);
        if (Data != null)
            writer.WriteObjectOffset(Data, options);
        else
            writer.WriteOffsetValue(0);
        
        writer.Write(ChildIndex);
        writer.Write(SiblingIndex);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }

    public void Add(CastNode item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Layer?.NotifyCastAdded(item);
        item.Layer = Layer;
        item.Parent = this;
        Children.Add(item);
    }

    public void Clear()
    {
        foreach (var child in this)
        {
            child.Parent = null;
            Layer?.NotifyCastRemoved(child);
        }

        Children.Clear();
    }

    public bool Contains(CastNode item)
    {
        return Children.Contains(item);
    }

    public void CopyTo(CastNode[] array, int arrayIndex)
    {
        Children.CopyTo(array, arrayIndex);
    }

    public bool Remove(CastNode item)
    {
        if (item?.Parent != this)
            return false;

        Children.Remove(item);
        item.Parent = null;
        Layer?.NotifyCastRemoved(item);
        return true;
    }

    public int IndexOf(CastNode item)
    {
        return Children.IndexOf(item);
    }

    public void Insert(int index, CastNode item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        item.Parent = this;
        Layer?.NotifyCastAdded(item);
        Children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Children[index].Parent = null;
        Layer?.NotifyCastRemoved(Children[index]);
        Children.RemoveAt(index);
    }

    public CastNode this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }

    public IEnumerator<CastNode> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Children).GetEnumerator();
    }

    public override string ToString()
    {
        return Name;
    }

    [Flags]
    public enum CastNodeAttribute : int
    {
        NullCast = 0,
        ImageCast = 1,
        SliceCast = 2,
        ReferenceCast = 3,
        BlendMaterialColor = 0x20,
        BlendDisplayFlag = 0x40,
        BlendIlluminationColor = 0x80,
        HasIlluminationColor = 0x100,
        LocalScale = 0x10000
    }
}