namespace SharpNeedle.Framework.HedgehogEngine.Mirage.MaterialData;
using SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource("hh/texture", @"\.texture$")]
public class Texture : SampleChunkResource
{
    public string? PictureName { get; set; }
    public byte TexCoordIndex { get; set; }
    public WrapMode WrapModeU { get; set; }
    public WrapMode WrapModeV { get; set; }
    public string? Type { get; set; }

    public Texture()
    {
        DataVersion = 1;
    }

    public override void Read(BinaryObjectReader reader)
    {
        PictureName = reader.ReadStringOffset();
        TexCoordIndex = reader.ReadByte();
        WrapModeU = (WrapMode)reader.ReadByte();
        WrapModeV = (WrapMode)reader.ReadByte();
        reader.Skip(1); // Padding
        Type = reader.ReadStringOffset();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, PictureName);
        writer.WriteByte(TexCoordIndex);
        writer.WriteByte((byte)WrapModeU);
        writer.WriteByte((byte)WrapModeV);
        writer.Align(4);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Type);
    }
}