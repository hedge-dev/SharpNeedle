namespace SharpNeedle.Framework.Ninja.Csd;

using FontCollection = CsdDictionary<Font>;

public class ProjectChunk : IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("nCPJ");
    public uint Signature { get; private set; } = BinSignature;
    public string? Name { get; set; }
    public TextureFormat TextureFormat { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public SceneNode? Root { get; set; }
    public FontCollection? Fonts { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        Field08 = reader.ReadUInt32();
        Field0C = reader.ReadUInt32();
        Root = reader.ReadObjectOffset<SceneNode>();
        Name = reader.ReadStringOffset();

        TextureFormat = (TextureFormat)reader.ReadInt32();
        Fonts = reader.ReadObjectOffset<FontCollection>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.WriteLittle(0); // Size

        SeekToken start = writer.At();
        writer.WriteUInt32(Field08);
        writer.WriteUInt32(Field0C);
        writer.WriteObjectOffset(Root);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteInt32((int)TextureFormat);
        writer.WriteObjectOffset(Fonts);
        writer.Flush();
        writer.Align(16);
        SeekToken end = writer.At();

        long size = (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.WriteLittle((int)size);

        end.Dispose();
    }
}

public enum TextureFormat
{
    Unknown = 0,
    Mirage = 0x2E44584C,
    RenderWare = 0x2E525754,
    NextNinja = 0x2E54584C,
}