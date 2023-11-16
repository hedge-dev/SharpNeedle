namespace SharpNeedle.SurfRide.Draw;
using System.IO;

public class ProjectChunk : ProjectNode, IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("SWPR");
    public uint Signature { get; private set; } = BinSignature;
    public uint Field0C { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        var start = reader.At();
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        if (options.Version >= 3)
            reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;

        reader.ReadAtOffset((int)start - 8 + reader.Read<int>(), () =>
        {
            base.Read(reader, options);
        });
        Field0C = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.Write<int>(0); // Size

        var start = writer.At();
        writer.Write(0x10); // Project Offset, untracked
        writer.Write(Field0C);
        base.Write(writer, options);

        writer.Flush();
        writer.Align(16);
        var end = writer.At();

        var size = (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.Write((int)size);

        end.Dispose();
    }
}

public class ProjectNode : IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public short Field06 { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }
    public float FrameRate { get; set; }
    public Camera Camera { get; set; }
    public UserData UserData { get; set; }
    public List<Scene> Scenes { get; set; } = new();
    public List<TextureList> TextureLists { get; set; } = new();
    public List<Font> Fonts { get; set; } = new();

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Data = this;

        Name = reader.ReadStringOffset();
        var sceneCount = reader.Read<ushort>();
        Field06 = reader.Read<short>();
        var texListCount = reader.Read<ushort>();
        var fontCount = reader.Read<ushort>();
        Scenes.AddRange(reader.ReadObjectArrayOffset<Scene, ChunkBinaryOptions>(options, sceneCount));
        TextureLists.AddRange(reader.ReadObjectArrayOffset<TextureList, ChunkBinaryOptions>(options, texListCount));
        Fonts.AddRange(reader.ReadObjectArrayOffset<Font, ChunkBinaryOptions>(options, fontCount));
        Camera = reader.ReadObject<Camera, ChunkBinaryOptions>(options);
        StartFrame = reader.Read<uint>();
        EndFrame = reader.Read<uint>();
        if (options.Version >= 1)
            FrameRate = reader.Read<float>();
        
        if (options.Version >= 3)
            reader.Align(8);
        
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        options.Data = this;

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write((ushort)Scenes.Count);
        writer.Write(Field06);
        writer.Write((ushort)TextureLists.Count);
        writer.Write((ushort)Fonts.Count);
        if (Scenes.Count != 0)
            writer.WriteObjectCollectionOffset(options, Scenes);
        else
            writer.WriteOffsetValue(0);
        
        if (TextureLists.Count != 0)
            writer.WriteOffsetValue(0x30);
        else
            writer.WriteOffsetValue(0);
        
        if (Fonts.Count != 0)
            writer.WriteObjectCollectionOffset(options, Fonts);
        else
            writer.WriteOffsetValue(0);
        
        writer.WriteObject(Camera, options);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        if (options.Version >= 1)
            writer.Write(FrameRate);
        
        if (options.Version >= 3)
            writer.Align(8);
        
        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }

    public Scene GetScene(string name)
    {
        return Scenes.Find(item => item.Name == name);
    }

    public TextureList GetTextureList(string name)
    {
        return TextureLists.Find(item => item.Name == name);
    }
    public Font GetFont(string name)
    {
        return Fonts.Find(item => item.Name == name);
    }
}