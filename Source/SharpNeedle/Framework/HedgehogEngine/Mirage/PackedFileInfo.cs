namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource(ResourceId, ResourceType.ArchiveInfo, @"\.pfi$")]
public class PackedFileInfo : SampleChunkResource
{
    public const string ResourceId = "hh/pfi";
    public List<File> Files { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        int fileCount = reader.ReadInt32();
        Files = new(fileCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < fileCount; i++)
            {
                Files.Add(reader.ReadObjectOffset<File>());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Files.Count);
        writer.WriteOffset(() =>
        {
            foreach (File file in Files)
            {
                writer.WriteObjectOffset(file);
            }
        });
    }

    public struct File : IBinarySerializable
    {
        public string? Name { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
            Offset = reader.ReadUInt32();
            Size = reader.ReadUInt32();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.WriteUInt32(Offset);
            writer.WriteUInt32(Size);
        }

        public override readonly string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}