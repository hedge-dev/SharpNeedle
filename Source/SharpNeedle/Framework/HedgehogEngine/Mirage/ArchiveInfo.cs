namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource("hh/arcinfo", @"\.arcinfo$")]
public class ArchiveInfo : SampleChunkResource
{
    public List<Entry> Entries { get; set; } = [];

    public ArchiveInfo()
    {
        DataVersion = 1;
    }

    public override void Read(BinaryObjectReader reader)
    {
        int entryCount = reader.Read<int>();
        Entries.AddRange(reader.ReadObjectArrayOffset<Entry>(entryCount));

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < entryCount; i++)
            {
                Entries[i].Field04 = reader.Read<byte>();
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Entries.Count);
        writer.WriteObjectCollectionOffset(Entries);

        writer.WriteOffset(() =>
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                writer.Write(Entries[i].Field04);
            }

            writer.Align(4);
        });
    }

    protected override void Reset()
    {
        Entries.Clear();
    }

    public class Entry : IBinarySerializable
    {
        public string? Name { get; set; }
        public byte Field04 { get; set; } // Priority?

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, -1, 1);
        }
    }
}
