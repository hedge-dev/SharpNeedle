namespace SharpNeedle.Framework.Glitter;

public class MaterialParameter : IBinarySerializable
{
    public string? Name { get; set; }
    public string? ShaderName { get; set; }
    public int TextureAddresMode { get; set; }
    public int Field2C { get; set; }
    public float Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public List<TextureData> TextureDatas { get; set; } = new(2);

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();

        for (int i = 0; i < 2; i++)
        {
            TextureDatas.Insert(i, reader.ReadObject<TextureData>());
        }

        int textureCount = reader.Read<int>();
        TextureAddresMode = reader.Read<int>();

        Field2C = reader.Read<int>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<int>();
        Field38 = reader.Read<int>();

        ShaderName = reader.ReadStringOffset();

        reader.Skip(reader.GetOffsetSize()); // VertexShaderDataOffset
        reader.Skip(reader.GetOffsetSize()); // PixelShaderDataOffset

        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
        Field54 = reader.Read<int>();
        Field58 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

        for (int i = 0; i < TextureDatas.Capacity; i++)
        {
            writer.WriteObject(TextureDatas[i]);
        }

        writer.Write(TextureDatas.Capacity);
        writer.Write(TextureAddresMode);

        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ShaderName);

        writer.Skip(writer.GetOffsetSize()); // VertexShaderDataOffset
        writer.Skip(writer.GetOffsetSize()); // PixelShaderDataOffset

        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
        writer.Write(Field54);
        writer.Write(Field58);
    }

    public struct TextureData : IBinarySerializable
    {
        public string? Name { get; set; }
        public float Field04 { get; set; }
        public int Field08 { get; set; }
        public int Field0C { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();

            Field04 = reader.Read<float>();

            Field08 = reader.Read<int>();
            Field0C = reader.Read<int>();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            writer.Write(Field04);

            writer.Write(Field08);
            writer.Write(Field0C);
        }
    }
}
