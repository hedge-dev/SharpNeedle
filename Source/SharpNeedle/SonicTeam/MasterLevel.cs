namespace SharpNeedle.SonicTeam; 

using SharpNeedle.BINA;

[NeedleResource("st/masterlevel", @"\.mlevel$")]
public class MasterLevel : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("LMEH");
    public int Field04 { get; set; }
    public List<LevelInfo> Levels { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;

        reader.EnsureSignature(Signature);

        Field04 = reader.Read<int>(); // Always 0? Potentially Padding or Version

        var levelCount = reader.ReadOffsetValue();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < levelCount; i++)
                Levels.Add(reader.ReadObjectOffset<LevelInfo>());
        });


        reader.Align(16);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.OffsetBinaryFormat = OffsetBinaryFormat.U64;

        writer.Write(Signature);
        writer.Write(Field04);

        Levels = Levels.OrderBy(o => o.Name).ToList();

        writer.Write<long>(Levels.Count);
        writer.WriteOffset(() =>
        {
            for (int i = 0; i < Levels.Count; i++)
                writer.WriteObjectOffset(Levels[i], 8);
        });

        writer.Align(16);
    }

    public class LevelInfo : IBinarySerializable
    {
        public string Name { get; set; }
        public List<FileInfo> Files { get; set; } = new();
        public List<FileInfo> Dependencies { get; set; } = new(); // Guessed
        public bool Field20 { get; set; }
        public bool Field21 { get; set; }
        public long Field28 { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            reader.Align(8);

            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            int fileCount = reader.Read<int>();
            int dependencyCount = reader.Read<int>();

            reader.ReadOffset(() =>
            {
                if (fileCount != 0)
                {
                    for (int i = 0; i < fileCount; i++)
                    {
                        reader.ReadOffset(() =>
                        {
                            Files.Add(reader.ReadObject<FileInfo>());
                        });
                    }
                }
            });

            reader.ReadOffset(() =>
            {
                if (dependencyCount != 0)
                {
                    for (int i = 0; i < dependencyCount; i++)
                    {
                        reader.ReadOffset(() =>
                        {
                            Dependencies.Add(reader.ReadObject<FileInfo>());
                        });
                    }
                }
            });

            Field20 = reader.Read<bool>();
            Field21 = reader.Read<bool>();
            Field28 = reader.ReadOffsetValue();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Align(8);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            Files = Files.OrderBy(o => o.Name).ToList();
            Dependencies = Dependencies.OrderBy(o => o.Name).ToList();

            writer.Write(Files.Count);
            writer.Write(Dependencies.Count);

            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    writer.WriteObjectOffset(Files[i], 8);
                }
            });

            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Dependencies.Count; i++)
                {
                    writer.WriteObjectOffset(Dependencies[i], 8);
                }
            });

            writer.Write(Field20);
            writer.Write(Field21);
            writer.Write(Field28);
        }
    }

    public class FileInfo : IBinarySerializable
    {
        public string Name { get; set; }
        public bool Field08 { get; set; }
        public long Field10 { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            reader.Align(8);

            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            reader.ReadOffset(() =>
            {
                Field08 = reader.Read<bool>();
            });

            Field10 = reader.ReadOffsetValue();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Align(8);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            writer.WriteOffset(() =>
            {
                writer.Write(Field08);
            });

            writer.Write(Field10);
        }
    }
}
