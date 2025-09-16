namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource(ResourceId, ResourceType.ArchiveInfo, @"\.pfi$")]
public class PackedFileInfo : SampleChunkResource
{
    public const string ResourceId = "hh/pfi";
    public List<File> Files { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        reader.Read(out int fileCount);
        Files = [];

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
        writer.Write(Files.Count);
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
            Offset = reader.Read<uint>();
            Size = reader.Read<uint>();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(Offset);
            writer.Write(Size);
        }

        public override readonly string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}