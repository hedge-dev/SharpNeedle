using SharpNeedle.Ninja.Csd;

namespace SharpNeedle.SurfRide.Draw;

public class Layer : IBinarySerializable<ChunkBinaryOptions>, IList<CastNode>
{
    public int Count => Children.Count;
    public bool IsReadOnly => false;

    public string Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public uint CurrentAnimationIndex { get; set; }
    public bool Is3D { get; set; }
    public List<CastNode> CastBuffer { get; set; } = new();
    public List<CastNode> Children { get; set; } = new();
    public List<Animation> Animations { get; set; } = new();
    public UserData UserData { get; set; }
    public IReadOnlyList<CastNode> Casts => CastBuffer;
    public Scene Scene { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Scene = (Scene)options.Data;
        options.Data = this;

        if (options.Version >= 3)
            reader.Align(8);
        
        Name = reader.ReadStringOffset();
        ID = reader.Read<int>();

        Flags = reader.Read<uint>();
        Is3D = (Flags & 0xF) == 1 ? true : false;

        var castCount = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);

        CastBuffer.AddRange(reader.ReadObjectArrayOffset<CastNode, ChunkBinaryOptions>(options, castCount));

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < castCount; i++)
                CastBuffer[i].Cell = reader.ReadObject<Cell, ChunkBinaryOptions>(options);
        });

        var tree = new TreeDescriptorNode[CastBuffer.Count];
        for (int i = 0; i < CastBuffer.Count; i++)
        {
            tree[i].ChildIndex = CastBuffer[i].ChildIndex;
            tree[i].SiblingIndex = CastBuffer[i].SiblingIndex;
        }

        /*if (CastBuffer.Count != 0)
            ParseTree(0);

        Children = new List<CastNode>(CastBuffer.Count);
        for (int i = 0; i < CastBuffer.Count; i++)
        {
            var cast = CastBuffer[i];
            cast.Priority = i;
            cast.Layer = this;
            if (cast.Parent == null)
                Children.Add(cast);
        }*/

        var animCount = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);

        Animations.AddRange(reader.ReadObjectArrayOffset<Animation, ChunkBinaryOptions>(options, animCount));
        CurrentAnimationIndex = reader.Read<uint>();
        if (options.Version >= 3)
            reader.Align(8);
        
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);


        void ParseTree(int idx, int p = -1)
        {
            var node = tree[idx];

            if (node.ChildIndex != -1)
            {
                CastBuffer[idx].Add(CastBuffer[node.ChildIndex]);
                ParseTree(node.ChildIndex, idx);
            }

            if (node.SiblingIndex != -1)
            {
                if (p != -1)
                    CastBuffer[p].Add(CastBuffer[node.SiblingIndex]);

                ParseTree(node.SiblingIndex, p);
            }
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        Scene = (Scene)options.Data;
        options.Data = this;
        
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(ID);
        writer.Write(Flags);
        writer.Write(Casts.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Casts.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, Casts);
            if (options.Version >= 4)
            {
                writer.WriteOffset(() =>
                {
                    foreach (var cast in CastBuffer)
                        writer.WriteObject(cast.Cell, options);
                }, 16);
            }
            else
            {
                writer.WriteOffset(() =>
                {
                    foreach (var cast in CastBuffer)
                        writer.WriteObject(cast.Cell, options);
                });
            }
        }
        else
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
        }
        
        writer.Write(Animations.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Animations.Count != 0)
            writer.WriteObjectCollectionOffset(options, Animations);
        else
            writer.WriteOffsetValue(0);
        
        writer.Write(CurrentAnimationIndex);
        if (options.Version >= 3)
            writer.Align(8);

        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }

    public CastNode GetCast(string name)
    {
        return CastBuffer.Find(item => item.Name == name);
    }

    public Animation GetAnimation(string name)
    {
        return Animations.Find(item => item.Name == name);
    }

    internal void NotifyCastAdded(CastNode cast)
    {
        TakeOwnership(cast);
    }

    internal void NotifyCastRemoved(CastNode cast)
    {
        Disown(cast);
    }

    internal void NotifyPriorityChanged(CastNode cast)
    {
        CastBuffer.Sort((x, y) => x.Priority - y.Priority);
    }

    private void TakeOwnership(CastNode cast)
    {
        cast.Layer?.Disown(cast);

        cast.Layer = this;
        CastBuffer.Add(cast);
        foreach (var child in cast)
            TakeOwnership(child);
    }

    private void Disown(CastNode cast)
    {
        CastBuffer.Remove(cast);
        foreach (var child in cast.Children)
            Disown(child);
    }

    public IEnumerator<CastNode> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Children).GetEnumerator();
    }

    public void Add(CastNode item)
    {
        Children.Add(item);
        TakeOwnership(item);

        if (HasPriority(item))
            CastBuffer.Sort((x, y) => x.Priority - y.Priority);

        bool HasPriority(CastNode cast)
        {
            if (cast.Priority != 0)
                return true;

            var hasPrio = false;
            foreach (var child in cast)
            {
                hasPrio = HasPriority(child);
                if (hasPrio)
                    break;
            }

            return hasPrio;
        }
    }

    public void Clear()
    {
        foreach (var cast in CastBuffer)
            cast.Layer = null;

        Children.Clear();
        CastBuffer.Clear();
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
        if (item?.Layer != this)
            return false;

        Children.Remove(item);
        Disown(item);
        return true;
    }

    public int IndexOf(CastNode item)
    {
        return Children.IndexOf(item);
    }

    public void Insert(int index, CastNode item)
    {
        TakeOwnership(item);
        Children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Disown(Children[index]);
        Children.RemoveAt(index);
    }


    public CastNode this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }

    public struct TreeDescriptorNode
    {
        public int ChildIndex;
        public int SiblingIndex;

        public TreeDescriptorNode()
        {
            ChildIndex = -1;
            SiblingIndex = -1;
        }

        public override string ToString()
        {
            return $"{{ ChildIndex: {ChildIndex}, SiblingIndex: {SiblingIndex} }}";
        }
    }
}

[Flags]
public enum LayerAttribute
{
    Is3D = 1,
}