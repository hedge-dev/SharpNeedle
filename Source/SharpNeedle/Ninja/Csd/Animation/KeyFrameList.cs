namespace SharpNeedle.Ninja.Csd.Animation;

public class KeyFrameList : IBinarySerializable, IList<KeyFrame>
{
    public uint Field00 { get; set; }
    public List<KeyFrame> Frames { get; set; }
    public KeyProperty Property { get; set; }

    public int Count => Frames.Count;
    public bool IsReadOnly => false;

    public void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<uint>();
        Frames = new List<KeyFrame>(reader.ReadObjectArrayOffset<KeyFrame>(reader.Read<int>()));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Frames.Count);

        writer.WriteObjectCollectionOffset(Frames);
    }

    public void ReadExtended(BinaryObjectReader reader)
    {
        reader.ReadOffset(() =>
        {
            reader.ReadOffset(() =>
            {
                reader.ReadOffset(() =>
                {
                    foreach (var frame in Frames)
                        frame.Correction = reader.ReadValueOffset<AspectRatioCorrection>();
                });
            });
        });
    }

    public void WriteExtended(BinaryObjectWriter writer)
    {
        writer.WriteOffset(() =>
        {
            writer.WriteOffset(() =>
            {
                writer.WriteOffset(() =>
                {
                    foreach (var frame in Frames)
                    {
                        writer.WriteValueOffset(frame.Correction ?? default);
                    }
                });
            });
        });
    }
    

    public IEnumerator<KeyFrame> GetEnumerator()
    {
        return Frames.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Frames).GetEnumerator();
    }

    public void Add(KeyFrame item)
    {
        Frames.Add(item);
    }

    public void Clear()
    {
        Frames.Clear();
    }

    public bool Contains(KeyFrame item)
    {
        return Frames.Contains(item);
    }

    public void CopyTo(KeyFrame[] array, int arrayIndex)
    {
        Frames.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyFrame item)
    {
        return Frames.Remove(item);
    }

    public int IndexOf(KeyFrame item)
    {
        return Frames.IndexOf(item);
    }

    public void Insert(int index, KeyFrame item)
    {
        Frames.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Frames.RemoveAt(index);
    }

    public KeyFrame this[int index]
    {
        get => Frames[index];
        set => Frames[index] = value;
    }
}