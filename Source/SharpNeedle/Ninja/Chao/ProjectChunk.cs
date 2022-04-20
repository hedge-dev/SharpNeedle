namespace SharpNeedle.Ninja.Chao;
using FontCollection = ChaoCollection<Font>;

public class ProjectChunk : IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("nCPJ");
    public uint Signature { get; private set; }
    public string Name { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public SceneNode Root { get; set; }
    public FontCollection Fonts { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Root = reader.ReadObjectOffset<SceneNode>();
        Name = reader.ReadStringOffset();

        var dxlSig = reader.Read<uint>(); // ???
        Fonts = reader.ReadObjectOffset<FontCollection>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Signature);
        writer.Write(0); // Size

        writer.Write(Field08);
        writer.Write(Field0C);
        writer.WriteObjectOffset(Root);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(BinaryHelper.MakeSignature<uint>("DXL."));
        writer.WriteObjectOffset(Fonts);
    }
}