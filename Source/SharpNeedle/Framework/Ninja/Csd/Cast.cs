﻿namespace SharpNeedle.Framework.Ninja.Csd;

using SharpNeedle.Framework.Ninja.Csd.Motions;
using SharpNeedle.Structs;

public class Cast : IBinarySerializable<Family>, IList<Cast>
{
    private int _priority;
    public int Count => Children.Count;
    public bool IsReadOnly => false;

    public Family? Family { get; internal set; }
    public Cast? Parent { get; set; }
    public string? Name { get; set; }
    public uint Field00 { get; set; }
    public EType Type { get; set; }
    public bool Enabled { get; set; }
    public Vector2 TopLeft { get; set; }
    public Vector2 BottomLeft { get; set; }
    public Vector2 TopRight { get; set; }
    public Vector2 BottomRight { get; set; }
    public BitSet<uint> Field2C { get; set; }
    public BitSet<uint> InheritanceFlags { get; set; }
    public BitSet<uint> MaterialFlags { get; set; }
    public string? Text { get; set; }
    public string? FontName { get; set; }
    public float FontKerning { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint Field58 { get; set; }
    public uint Field5C { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Position { get; set; }
    public uint Field70 { get; set; }
    public CastInfo Info { get; set; }
    public int[] SpriteIndices { get; set; } = [];
    public List<Cast> Children { get; set; } = [];

    public int Priority
    {
        get => _priority;
        set
        {
            int oldPriority = _priority;
            _priority = value;

            if (oldPriority != _priority)
            {
                Family?.NotifyPriorityChanged(this);
            }
        }
    }

    public void Read(BinaryObjectReader reader, Family family)
    {
        Field00 = reader.Read<uint>();
        Type = reader.Read<EType>();
        Enabled = reader.Read<uint>() != 0;

        TopLeft = reader.Read<Vector2>();
        BottomLeft = reader.Read<Vector2>();
        TopRight = reader.Read<Vector2>();
        BottomRight = reader.Read<Vector2>();

        Field2C = reader.Read<BitSet<uint>>();
        Info = reader.ReadObjectOffset<CastInfo>();
        InheritanceFlags = reader.Read<BitSet<uint>>();
        MaterialFlags = reader.Read<BitSet<uint>>();
        SpriteIndices = reader.ReadArrayOffset<int>(reader.Read<int>());

        Text = reader.ReadStringOffset();
        FontName = reader.ReadStringOffset();
        FontKerning = reader.ReadSingle();

        if (family.Scene == null)
        {
            throw new InvalidOperationException("Family scene is null!");
        }

        if (family.Scene.Version >= 3)
        {
            Width = reader.Read<uint>();
            Height = reader.Read<uint>();
            Field58 = reader.Read<uint>();
            Field5C = reader.Read<uint>();

            Origin = reader.Read<Vector2>();
            Position = reader.Read<Vector2>();
            Field70 = reader.Read<uint>();
        }
    }

    public void Write(BinaryObjectWriter writer, Family? family)
    {
        writer.Write(Field00);
        writer.Write(Type);
        writer.Write(Convert.ToInt32(Enabled));

        writer.Write(TopLeft);
        writer.Write(BottomLeft);
        writer.Write(TopRight);
        writer.Write(BottomRight);

        writer.Write(Field2C);

        // Info is written after the sprite indices, despite appearing in the struct earlier.
        long infoOffset = writer.Position;
        writer.OffsetHandler.RegisterOffsetPosition(infoOffset);
        writer.WriteOffsetValue(0);

        writer.Write(InheritanceFlags);
        writer.Write(MaterialFlags);

        writer.Write(SpriteIndices.Length);
        writer.WriteOffset(() =>
        {
            writer.WriteArray(SpriteIndices);

            long offset = writer.Position;
            using (SeekToken token = writer.At(infoOffset, SeekOrigin.Begin))
            {
                writer.WriteOffsetValue(writer.OffsetHandler.CalculateOffset(offset, writer.OffsetHandler.OffsetOrigin));
            }

            writer.WriteObject(Info);
        });

        if (Text != null)
        { 
            writer.WriteOffset(() =>
            { 
                writer.WriteString(StringBinaryFormat.NullTerminated, Text);
                writer.Write<byte>(0);
            });
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (FontName != null)
        {
            writer.WriteOffset(() =>
            {
                writer.WriteString(StringBinaryFormat.NullTerminated, FontName);
                writer.Write<byte>(0);
            });
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.Write(FontKerning);

        family ??= Family;

        if (family == null)
        {
            throw new InvalidOperationException("family is null!");
        }

        if (family.Scene == null)
        {
            throw new InvalidOperationException("Family scene is null!");
        }

        if (family.Scene.Version >= 3)
        {
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Field58);
            writer.Write(Field5C);

            writer.Write(Origin);
            writer.Write(Position);
            writer.Write(Field70);
        }
    }

    public void AttachMotion(CastMotion motion)
    {
        motion.Cast = this;
    }

    public void Add(Cast item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        Family?.NotifyCastAdded(item);
        item.Family = Family;
        item.Parent = this;
        Children.Add(item);
    }

    public void Clear()
    {
        foreach (Cast child in this)
        {
            child.Parent = null;
            Family?.NotifyCastRemoved(child);
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
        {
            return false;
        }

        Children.Remove(item);
        item.Parent = null;
        Family?.NotifyCastRemoved(item);
        return true;
    }

    public int IndexOf(Cast item)
    {
        return Children.IndexOf(item);
    }

    public void Insert(int index, Cast item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.Parent = this;
        Family?.NotifyCastAdded(item);
        Children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Children[index].Parent = null;
        Family?.NotifyCastRemoved(Children[index]);
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
        return Name ?? string.Empty;
    }
    public enum EType : int
    {
        Null,
        Sprite,
        Font
    }
}
