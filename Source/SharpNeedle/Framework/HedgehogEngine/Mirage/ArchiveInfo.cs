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
        int entryCount = reader.ReadInt32();
        Entries.AddRange(reader.ReadObjectArrayOffset<Entry>(entryCount));

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < entryCount; i++)
            {
                Entries[i].Field04 = reader.ReadByte();
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Entries.Count);
        writer.WriteObjectCollectionOffset(Entries);

        writer.WriteOffset(() =>
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                writer.WriteByte(Entries[i].Field04);
            }

            writer.Align(4);
        });
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
