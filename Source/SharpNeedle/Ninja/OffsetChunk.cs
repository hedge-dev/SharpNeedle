namespace SharpNeedle.Ninja;
using System.IO;

public class OffsetChunk : IChunk, IList<int>
{
    public uint Signature { get; private set; } = BinaryHelper.MakeSignature<uint>("NOF0");

    public List<int> Offsets { get; set; } = new();
    public int Count => Offsets.Count;
    public bool IsReadOnly => false;

    public int BinarySize => AlignmentHelper.Align(16 + (Offsets.Count * sizeof(int)), 16);

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        
        var count = reader.Read<int>();
        reader.Skip(4); // Runtime flags
        Offsets.AddRange(reader.ReadArray<int>(count));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.WriteLittle(Signature);
        writer.Write(BinarySize - 8); // Size excluding header
        
        writer.Write(Offsets.Count);
        writer.Write(0); // Runtime flags, make this 0 to kill the game
        writer.WriteCollection(Offsets);
        writer.Align(16);
    }

    public IEnumerator<int> GetEnumerator()
    {
        return Offsets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Offsets).GetEnumerator();
    }

    public void Add(int item)
    {
        Offsets.Add(item);
    }

    public void Clear()
    {
        Offsets.Clear();
    }

    public bool Contains(int item)
    {
        return Offsets.Contains(item);
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        Offsets.CopyTo(array, arrayIndex);
    }

    public bool Remove(int item)
    {
        return Offsets.Remove(item);
    }

    public int IndexOf(int item)
    {
        return Offsets.IndexOf(item);
    }

    public void Insert(int index, int item)
    {
        Offsets.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Offsets.RemoveAt(index);
    }

    public int this[int index]
    {
        get => Offsets[index];
        set => Offsets[index] = value;
    }
}