namespace SharpNeedle.SonicTeam; 

using SharpNeedle.BINA;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

[NeedleResource("st/masterlevel", @"\.mlevel$")]
public class MasterLevel : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("LMEH");
    public List<LevelInfo> Levels { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;

        reader.EnsureSignature(Signature);

        reader.Align(reader.GetOffsetSize()); // 4 empty bytes, always 0(?). Very Likely Padding or Version

        var levelCount = reader.ReadOffsetValue();
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < levelCount; i++)
                Levels.Add(reader.ReadObjectOffset<LevelInfo>());

            reader.Skip(8); // Potential Runtime Field
        });

        reader.Align(16);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.OffsetBinaryFormat = OffsetBinaryFormat.U64;
        Levels = Levels.OrderBy(o => o.Name).ToList();

        writer.Write(Signature);

        writer.Align(writer.GetOffsetSize());

        writer.Write<long>(Levels.Count);
        writer.WriteOffset(() =>
        {
            for (int i = 0; i < Levels.Count; i++)
                writer.WriteObjectOffset(Levels[i], writer.GetOffsetSize());

            writer.Skip(8); // Potential Runtime Field
        });

        writer.Align(16);
    }

    public class LevelInfo : IBinarySerializable
    {
        public string Name { get; set; }
        public bool HasFiles { get; set; }
        public bool Field20 { get; set; }
        public List<FileInfo> Files { get; set; } = new();
        public List<FileInfo> Dependencies { get; set; } = new(); // Guessed

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
                            Dependencies.Add(reader.ReadObject<FileInfo, bool>(i + 1 == dependencyCount));
                        });
                    }
                }
            });

            Field20 = reader.Read<bool>();
            HasFiles = reader.Read<bool>();

            reader.Align(8);

            reader.Skip(8); // Potential Runtime Field

            reader.Align(8);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Align(8);

            Files = Files.OrderBy(o => o.Name).ToList();
            Dependencies = Dependencies.OrderBy(o => o.Name).ToList();

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            writer.Write(Files.Count);
            writer.Write(Dependencies.Count);

            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    writer.WriteObjectOffset(Files[i], writer.GetOffsetSize());
                }
            });

            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Dependencies.Count; i++)
                {
                    writer.WriteObjectOffset(Dependencies[i], i + 1 == Dependencies.Count, writer.GetOffsetSize());
                }
            });

            writer.Write(Field20);
            writer.Write(HasFiles);

            writer.Align(8);

            writer.Skip(8); // Potential Runtime Field

            writer.Align(8);
        }
    }

    public class FileInfo : IBinarySerializable<bool>
    {
        public string Name { get; set; }
        public bool Field08 { get; set; }
        public string NextDependencyName { get; set; }
        public FileInfo NextFileInfo { get; set; }

        public void Read(BinaryObjectReader reader, bool isLast)
        {
            reader.Align(8);

            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            reader.ReadOffset(() =>
            {
                Field08 = reader.Read<bool>();
            });

            long offset = reader.ReadOffsetValue();
            if (offset != 0)
            {
                reader.ReadAtOffset(offset, () =>
                {
                    if (isLast)
                    {
                        NextFileInfo = reader.ReadObject<FileInfo>();
                    }
                    else
                    {
                        NextDependencyName = reader.ReadString(StringBinaryFormat.NullTerminated);
                    }
                });
            }
        }

        public void Write(BinaryObjectWriter writer, bool isLast)
        {
            writer.Align(8);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            writer.WriteOffset(() =>
            {
                writer.Write(Field08);
            });

            if (String.IsNullOrEmpty(NextDependencyName))
            {
                if (NextFileInfo == null)
                {
                    writer.WriteOffsetValue(0);
                }
                else
                {
                    writer.WriteObjectOffset(NextFileInfo);
                }
            }
            else
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, NextDependencyName);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct UnknownUnion
    {
        [FieldOffset(0)] public string Name;
        [FieldOffset(0)] public FileInfo FileInfo;

        public UnknownUnion(string value) : this() => Name = value;
        public UnknownUnion(FileInfo value) : this() => FileInfo = value;

        public void Set(string value) => Name = value;
        public void Set(FileInfo value) => FileInfo = value;

        public static implicit operator UnknownUnion(string value) => new(value);
        public static implicit operator UnknownUnion(FileInfo value) => new(value);

        public static implicit operator string(UnknownUnion value) => value.Name;
        public static implicit operator FileInfo(UnknownUnion value) => value.FileInfo;
    }
}
