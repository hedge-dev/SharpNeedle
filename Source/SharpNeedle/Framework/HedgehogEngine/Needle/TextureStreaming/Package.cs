namespace SharpNeedle.Framework.HedgehogEngine.Needle.TextureStreaming;

public class Package : ResourceBase, IBinarySerializable, IStreamable
{
    public struct EntryInfo : IBinarySerializable
    {
        private string _name;

        public int Hash { get; set; }
        public int BlockIndex { get; set; }
        public int MipLevels { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public string Name
        {
            readonly get => _name;
            set
            {
                _name = value;
                Hash = ComputeHash(value);
            }
        }

        public void Read(BinaryObjectReader reader)
        {
            Hash = reader.ReadInt32();
            BlockIndex = reader.ReadInt32();
            MipLevels = reader.ReadInt32();
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            _name = reader.ReadStringOffset();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteInt32(Hash);
            writer.WriteInt32(BlockIndex);
            writer.WriteInt32(MipLevels);
            writer.WriteUInt16(Width);
            writer.WriteUInt16(Height);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, alignment: 1);
        }

        public static int ComputeHash(string str)
        {
            int hash = 0;

            foreach(char c in str)
            {
                hash = hash * 31 + c;
            }

            return hash & 0x7FFFFFFF;
        }

        public override readonly string ToString()
        {
            return $"{Width}x{Height}(x{MipLevels}) , \"{Name}\"";
        }
    }


    public List<EntryInfo> Entries { get; set; } = [];

    public List<DataBlock> Blocks { get; set; } = [];

    public Stream BaseStream { get; private set; }


    public EntryInfo FindEntry(string name)
    {
        if(!TryFindEntry(name, out EntryInfo result))
        {
            throw new KeyNotFoundException($"Package has no entry with the name \"{name}\"!");
        }

        return result;
    }

    public bool TryFindEntry(string name, out EntryInfo entry)
    {
        int hash = EntryInfo.ComputeHash(name);

        foreach(EntryInfo currentEntry in Entries)
        {
            if(currentEntry.Hash == hash)
            {
                entry = currentEntry;
                return true;
            }
        }

        entry = default;
        return false;
    }


    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;
        BaseStream = file.Open();

        using BinaryObjectReader reader = new(BaseStream, StreamOwnership.Retain, Endianness.Little)
        {
            OffsetBinaryFormat = OffsetBinaryFormat.U64
        };

        Read(reader);
    }

    public override void Write(IFile file)
    {
        using Stream stream = file.Open(FileAccess.Write);
        BinaryObjectWriter writer = new(stream, StreamOwnership.Transfer, Endianness.Little, Encoding.ASCII)
        {
            OffsetBinaryFormat = OffsetBinaryFormat.U64
        };

        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        if(reader.ReadInt32() != 0x4E545350)
        {
            throw new InvalidDataException("Invalid signature, expected NTSP");
        }

        if(reader.ReadInt32() != 1)
        {
            throw new InvalidDataException("Invalid version, expected 1");
        }

        int entryCount = reader.ReadInt32();
        int blockCount = reader.ReadInt32();
        reader.Skip(8); // header size

        for(int i = 0; i < entryCount; i++)
        {
            Entries.Add(reader.ReadObject<EntryInfo>());
        }

        for(int i = 0; i < blockCount; i++)
        {
            Blocks.Add(reader.ReadObject<DataBlock>());
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(0x4E545350);
        writer.WriteInt32(1);

        writer.WriteInt32(Entries.Count);
        writer.WriteInt32(Blocks.Count);
        long headerSizePosition = writer.Position;
        writer.WriteNulls(8); // header size placeholder

        foreach(EntryInfo entry in Entries)
        {
            entry.Write(writer);
        }

        long blockStartPosition = writer.Position;
        // can't write those yet, otherwise header size is unobtainable
        writer.WriteNulls(Blocks.Count * 16);

        // Header size is from start to file to start of blockdata
        // and includes the strings, so we flush to write string offsets
        // to obtain the header size, and write block data after
        writer.Flush();
        writer.Seek(headerSizePosition, SeekOrigin.Begin);
        writer.WriteInt64(writer.Length);

        writer.Seek(blockStartPosition, SeekOrigin.Begin);

        foreach(DataBlock block in Blocks)
        {
            block.Write(writer);
        }

        writer.Seek(0, SeekOrigin.End);
        writer.Flush();
    }


    public void LoadToMemory()
    {
        foreach(DataBlock block in Blocks)
        {
            block.EnsureData();
        }

        BaseFile?.Dispose();
    }

}