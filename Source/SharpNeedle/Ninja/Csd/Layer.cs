namespace SharpNeedle.Ninja.Csd;
using Animation;

public class Layer : IBinarySerializable, IList<Cast>
{
    public int Count => Children.Count;
    public bool IsReadOnly => false;

    public List<Cast> Casts { get; private set; } = new();
    private List<Cast> Children { get; set; } = new();

    public void AttachMotion(LayerMotion motion)
    {
        motion.Layer = this;
        motion.OnAttach(this);
    }

    public void Read(BinaryObjectReader reader)
    {
        Casts = reader.ReadObject<BinaryList<BinaryPointer<Cast>>>().Unwind();
        var root = reader.Read<int>();
        var tree = reader.ReadArrayOffset<TreeDescriptorNode>(Casts.Count);
        if (Casts.Count != 0)
            ParseTree(root);

        Children = new List<Cast>(Casts.Count);
        foreach (var cast in Casts)
        {
            cast.Layer = this;
            if (cast.Parent == null)
                Children.Add(cast);
        }

        void ParseTree(int idx, int p = -1)
        {
            var node = tree[idx];

            if (node.ChildIndex != -1)
            {
                Casts[idx].Add(Casts[node.ChildIndex]);
                ParseTree(node.ChildIndex, idx);
            }

            if (node.NextIndex != -1)
            {
                if (p != -1)
                    Casts[p].Add(Casts[node.NextIndex]);
                
                ParseTree(node.NextIndex, p);
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Casts.Count);
        writer.WriteOffset(() =>
        {
            foreach (var cast in Casts)
                writer.WriteObjectOffset(cast);
        });
        
        writer.Write<int>(0);
        
        var nodes = new TreeDescriptorNode[Casts.Count];

        // Initialise items because, the runtime doesn't
        for (int i = 0; i < nodes.Length; i++)
            nodes[i] = new TreeDescriptorNode();
        
        BuildTree(Children);

        writer.WriteArrayOffset(nodes);

        void BuildTree(IList<Cast> casts)
        {
            for (int i = 0; i < casts.Count; i++)
            {
                var cast = casts[i];
                var idx = Casts.IndexOf(cast);
                
                if (cast.Children.Count > 0)
                    nodes[idx].ChildIndex = Casts.IndexOf(cast.Children[0]);

                if (i + 1 < casts.Count)
                    nodes[idx].NextIndex = Casts.IndexOf(casts[i + 1]);

                BuildTree(cast);
            }
        }
    }

    internal void NotifyCastAdded(Cast cast)
    {
        TakeOwnership(cast);
    }

    internal void NotifyCastRemoved(Cast cast)
    {
        Disown(cast);
    }

    private void TakeOwnership(Cast cast)
    {
        cast.Layer?.Disown(cast);
        
        cast.Layer = this;
        Casts.Add(cast);
        foreach (var child in cast)
            TakeOwnership(child);
    }

    private void Disown(Cast cast)
    {
        Casts.Remove(cast);
        foreach (var child in cast.Children)
            Disown(child);
    }

    public IEnumerator<Cast> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Children).GetEnumerator();
    }

    public void Add(Cast item)
    {
        Children.Add(item);
        TakeOwnership(item);
    }

    public void Clear()
    {
        foreach (var cast in Casts)
            cast.Layer = null;
        
        Children.Clear();
        Casts.Clear();
    }

    public bool Contains(Cast item)
    {
        return Children.Contains(item);
    }

    public void CopyTo(Cast[] array, int arrayIndex)
    {
        Children.CopyTo(array, arrayIndex);
    }

    public bool Remove(Cast item)
    {
        if (item?.Layer != this)
            return false;

        Children.Remove(item);
        Disown(item);
        return true;
    }

    public int IndexOf(Cast item)
    {
        return Children.IndexOf(item);
    }

    public void Insert(int index, Cast item)
    {
        TakeOwnership(item);
        Children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Disown(Children[index]);
        Children.RemoveAt(index);
    }

    public Cast this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }

    internal struct TreeDescriptorNode
    {
        public int ChildIndex;
        public int NextIndex;
        
        public TreeDescriptorNode()
        {
            ChildIndex = -1;
            NextIndex = -1;
        }

        public override string ToString()
        {
            return $"{{ ChildIndex: {ChildIndex}, NextIndex: {NextIndex} }}";
        }
    }
}