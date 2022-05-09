namespace SharpNeedle.Ninja.Csd;
using Animation;

public class Cast : IBinarySerializable, IList<Cast>
{
    public int Count => Children.Count;
    public bool IsReadOnly => false;

    public Layer Layer { get; internal set; }
    public Cast Parent { get; set; }
    public string Name { get; set; }
    public uint Field00 { get; set; }
    public uint Field04 { get; set; }
    public bool Enabled { get; set; }
    public Vector2 TopLeft { get; set; }
    public Vector2 BottomLeft { get; set; }
    public Vector2 TopRight { get; set; }
    public Vector2 BottomRight { get; set; }
    public uint Field2C { get; set; }
    public BitSet<uint> InheritanceFlags { get; set; }
    public uint Field38 { get; set; }
    public uint Field3C { get; set; }
    public string FontCharacters { get; set; }
    public string FontName { get; set; }
    public uint Field4C { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint Field58 { get; set; }
    public uint Field5C { get; set; }
    public Vector2 Offset { get; set; }
    public float Field68 { get; set; }
    public float Field6C { get; set; }
    public uint Field70 { get; set; }
    public CastInfo Info { get; set; }
    public CastMaterialInfo Material { get; set; }
    public List<Cast> Children { get; set; } = new();

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<uint>();
        Enabled = reader.Read<uint>() != 0;
        
        TopLeft = reader.Read<Vector2>();
        BottomLeft = reader.Read<Vector2>();
        TopRight = reader.Read<Vector2>();
        BottomRight = reader.Read<Vector2>();
        
        Field2C = reader.Read<uint>();
        Info = reader.ReadValueOffset<CastInfo>();
        InheritanceFlags = reader.Read<BitSet<uint>>();
        Field38 = reader.Read<uint>();
        Field3C = reader.Read<uint>();
        Material = reader.ReadObjectOffset<CastMaterialInfo>();

        FontCharacters = reader.ReadStringOffset();
        FontName = reader.ReadStringOffset();
        
        Field4C = reader.Read<uint>();
        Width = reader.Read<uint>();
        Height = reader.Read<uint>();
        Field58 = reader.Read<uint>();
        Field5C = reader.Read<uint>();
        
        Offset = reader.Read<Vector2>();
        Field68 = reader.Read<float>();
        Field6C = reader.Read<float>();
        Field70 = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Convert.ToInt32(Enabled));

        writer.Write(TopLeft);
        writer.Write(BottomLeft);
        writer.Write(TopRight);
        writer.Write(BottomRight);

        writer.Write(Field2C);
        writer.WriteValueOffset(Info);
        writer.Write(InheritanceFlags);
        writer.Write(Field38);
        writer.Write(Field3C);

        writer.WriteObjectOffset(Material);

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, FontCharacters);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, FontName);

        writer.Write(Field4C);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Field58);
        writer.Write(Field5C);

        writer.Write(Offset);
        writer.Write(Field68);
        writer.Write(Field6C);
        writer.Write(Field70);
    }

    public void AttachMotion(CastMotion motion)
    {
        motion.Cast = this;
    }

    public void Add(Cast item)
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
        if (item?.Parent != this)
            return false;

        Children.Remove(item);
        item.Parent = null;
        Layer?.NotifyCastRemoved(item);
        return true;
    }

    public int IndexOf(Cast item)
    {
        return Children.IndexOf(item);
    }

    public void Insert(int index, Cast item)
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

    public Cast this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }

    public IEnumerator<Cast> GetEnumerator()
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
}