namespace SharpNeedle.Framework.SurfRide.Draw;

public class ProjectChunk : ProjectNode, IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("SWPR");
    public uint Signature { get; private set; } = BinSignature;
    public uint Field0C { get; set; }

    public new void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        SeekToken start = reader.At();
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        if (options.Version >= 3)
        {
            reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
        }

        reader.ReadAtOffset((int)start - 8 + reader.ReadInt32(), () => base.Read(reader, options));
        Field0C = reader.ReadUInt32();
    }

    public new void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.WriteInt32(0); // Size

        SeekToken start = writer.At();
        writer.WriteInt32(0x10); // Project Offset, untracked
        writer.WriteUInt32(Field0C);
        base.Write(writer, options);

        writer.Flush();
        writer.Align(16);
        SeekToken end = writer.At();

        long size = (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.WriteInt32((int)size);

        end.Dispose();
    }
}

public class ProjectNode : IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public short Field06 { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public float FrameRate { get; set; }
    public Camera? Camera { get; set; }
    public UserData? UserData { get; set; }
    public List<Scene> Scenes { get; set; } = [];
    public List<TextureList> TextureLists { get; set; } = [];
    public List<Font> Fonts { get; set; } = [];

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Name = reader.ReadStringOffset();
        ushort sceneCount = reader.ReadUInt16();
        Field06 = reader.ReadInt16();
        ushort texListCount = reader.ReadUInt16();
        ushort fontCount = reader.ReadUInt16();
        Scenes.AddRange(reader.ReadObjectArrayOffset<Scene, ChunkBinaryOptions>(options, sceneCount));
        TextureLists.AddRange(reader.ReadObjectArrayOffset<TextureList, ChunkBinaryOptions>(options, texListCount));
        Fonts.AddRange(reader.ReadObjectArrayOffset<Font, ChunkBinaryOptions>(options, fontCount));
        Camera = reader.ReadObject<Camera, ChunkBinaryOptions>(options);
        StartFrame = reader.ReadUInt32();
        EndFrame = reader.ReadUInt32();
        if (options.Version >= 1)
        {
            FrameRate = reader.ReadSingle();
        }

        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
        {
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (Camera == null)
        {
            throw new InvalidOperationException("Camera is null");
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteUInt16((ushort)Scenes.Count);
        writer.WriteInt16(Field06);
        writer.WriteUInt16((ushort)TextureLists.Count);
        writer.WriteUInt16((ushort)Fonts.Count);
        if (Scenes.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, Scenes);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (TextureLists.Count != 0)
        {
            writer.WriteOffsetValue(0x30);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (Fonts.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, Fonts);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.WriteObject(Camera, options);
        writer.WriteUInt32(StartFrame);
        writer.WriteUInt32(EndFrame);
        if (options.Version >= 1)
        {
            writer.WriteSingle(FrameRate);
        }

        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (UserData != null)
        {
            writer.WriteObjectOffset(UserData, options);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }

    public Scene? GetScene(string name)
    {
        return Scenes.Find(item => item.Name == name);
    }

    public TextureList? GetTextureList(string name)
    {
        return TextureLists.Find(item => item.Name == name);
    }
    public Font? GetFont(string name)
    {
        return Fonts.Find(item => item.Name == name);
    }
}