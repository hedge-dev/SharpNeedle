namespace SharpNeedle.HedgehogEngine.Mirage;

public class PackedFileInfo : SampleChunkResource
{
    public List<File> Files { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        reader.Read(out int fileCount);
        Files = new List<File>();

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < fileCount; i++)
                Files.Add(reader.ReadObjectOffset<File>());
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Files.Count);
        writer.WriteOffset(() =>
        {
            foreach (var file in Files)
                writer.WriteObjectOffset(file);
        });
    }

    public struct File : IBinarySerializable
    {
        public string Name { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
            Offset = reader.Read<uint>();
            Size = reader.Read<uint>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(Offset);
            writer.Write(Size);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}