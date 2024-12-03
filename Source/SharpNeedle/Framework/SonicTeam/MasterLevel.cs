namespace SharpNeedle.Framework.SonicTeam;

using SharpNeedle.Framework.BINA;

[NeedleResource("st/masterlevel", @"\.mlevel$")]
public class MasterLevel : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("LMEH");
    public List<LevelInfo> Levels { get; set; } = [];

    public MasterLevel()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);

        reader.Align(reader.GetOffsetSize()); // 4 empty bytes, always 0(?). Very Likely Padding or Version

        long levelCount = reader.ReadOffsetValue();
        reader.ReadOffset(() =>
        {
            LevelInfoBinaryOptions options = new();

            for(int i = 0; i < levelCount; i++)
            {
                options.IsLast = i + 1 == levelCount;
                Levels.Add(reader.ReadObjectOffset<LevelInfo, LevelInfoBinaryOptions>(options));
            }

            reader.Skip(8); // Potential Runtime Field
        });

        reader.Align(16);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        Levels = [.. Levels.OrderBy(o => o.Name)];

        writer.Write(Signature);

        writer.Align(writer.GetOffsetSize());

        writer.Write<long>(Levels.Count);
        writer.WriteOffset(() =>
        {
            LevelInfoBinaryOptions options = new();

            for(int i = 0; i < Levels.Count; i++)
            {
                options.IsLast = i + 1 == Levels.Count;
                writer.WriteObjectOffset(Levels[i], options, writer.GetOffsetSize());
            }

            writer.Skip(8); // Potential Runtime Field
        });

        writer.Align(16);
    }

    public class LevelInfo : IBinarySerializable<LevelInfoBinaryOptions>
    {
        public string? Name { get; set; }
        public bool HasFiles { get; set; }
        public bool Field20 { get; set; }
        public List<FileInfo> Files { get; set; } = [];
        public List<FileInfo> Dependencies { get; set; } = []; // Guessed

        public void Read(BinaryObjectReader reader, LevelInfoBinaryOptions options)
        {
            reader.Align(8);

            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            int fileCount = reader.Read<int>();
            int dependencyCount = reader.Read<int>();

            reader.ReadOffset(() =>
            {
                if(fileCount != 0)
                {
                    for(int i = 0; i < fileCount; i++)
                    {
                        reader.ReadOffset(() => Files.Add(reader.ReadObject<FileInfo>()));
                    }
                }
            });

            reader.ReadOffset(() =>
            {
                if(dependencyCount != 0)
                {
                    FileInfoBinaryOptions fileOptions = new()
                    {
                        IsDependency = true,
                        IsLastLevel = options.IsLast
                    };

                    for(int i = 0; i < dependencyCount; i++)
                    {
                        fileOptions.IsLast = i + 1 == dependencyCount;

                        reader.ReadOffset(() => Dependencies.Add(reader.ReadObject<FileInfo, FileInfoBinaryOptions>(fileOptions)));
                    }
                }
            });

            Field20 = reader.Read<bool>();
            HasFiles = reader.Read<bool>();

            reader.Align(8);

            reader.Skip(8); // Potential Runtime Field

            reader.Align(8);
        }

        public void Write(BinaryObjectWriter writer, LevelInfoBinaryOptions options)
        {
            writer.Align(8);

            Files = [.. Files.OrderBy(o => o.Name)];
            Dependencies = [.. Dependencies.OrderBy(o => o.Name)];

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, writer.GetOffsetSize());

            writer.Write(Files.Count);
            writer.Write(Dependencies.Count);

            writer.WriteOffset(() =>
            {
                for(int i = 0; i < Files.Count; i++)
                {
                    writer.WriteObjectOffset(Files[i], writer.GetOffsetSize());
                }
            });

            writer.WriteOffset(() =>
            {
                FileInfoBinaryOptions fileOptions = new()
                {
                    IsDependency = true,
                    IsLastLevel = options.IsLast
                };

                for(int i = 0; i < Dependencies.Count; i++)
                {
                    fileOptions.IsLast = i + 1 == Dependencies.Count;

                    writer.WriteObjectOffset(Dependencies[i], fileOptions, writer.GetOffsetSize());
                }
            });

            writer.Write(Field20);
            writer.Write(HasFiles);

            writer.Align(8);

            writer.Skip(8); // Potential Runtime Field

            writer.Align(8);
        }
    }

    public class FileInfo : IBinarySerializable<FileInfoBinaryOptions>
    {
        public string? Name { get; set; }
        public bool Field08 { get; set; }
        public string? NextDependencyName { get; set; }
        public FileInfo? NextFileInfo { get; set; }

        public void Read(BinaryObjectReader reader, FileInfoBinaryOptions options)
        {
            reader.Align(8);

            Name = reader.ReadStringOffset();

            if(options.IsDependency)
            {
                reader.ReadOffsetValue();
            }
            else
            {
                reader.ReadOffset(() => Field08 = reader.Read<bool>());
            }

            if(!options.IsLastLevel)
            {
                long offset = reader.ReadOffsetValue();
                if(offset != 0)
                {
                    reader.ReadAtOffset(offset, () =>
                    {
                        if(options.IsLast)
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
        }

        public void Write(BinaryObjectWriter writer, FileInfoBinaryOptions options)
        {
            writer.Align(8);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, writer.GetOffsetSize());

            if(options.IsDependency)
            {
                writer.WriteOffsetValue(0);
            }
            else
            {
                writer.WriteOffset(() => writer.Write(Field08));
            }

            if(!options.IsLastLevel)
            {
                if(string.IsNullOrEmpty(NextDependencyName))
                {
                    if(NextFileInfo == null)
                    {
                        writer.WriteOffsetValue(0);
                    }
                    else
                    {
                        writer.WriteObjectOffset(NextFileInfo, writer.GetOffsetSize());
                    }
                }
                else
                {
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, NextDependencyName, writer.GetOffsetSize());
                }
            }
        }
    }

    public struct LevelInfoBinaryOptions
    {
        public bool IsLast;
    }

    public struct FileInfoBinaryOptions
    {
        public bool IsLast;
        public bool IsLastLevel;
        public bool IsDependency;
    }
}
