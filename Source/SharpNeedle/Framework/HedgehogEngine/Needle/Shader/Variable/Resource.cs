namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader.Variable;

public enum ResourceType : int
{
    Buffer = 0,
    Texture1D = 1,
    Texture1DArray = 2,
    Texture2D = 3,
    Texture2DArray = 4,
    Texture2DMultisampled = 5,
    Texture2DMultisampledArray = 6,
    Texture3D = 7,
    TextureCube = 8,
    TextureCubeArray = 9,
    BufferExtended = 10
}

public struct Resource : IBinarySerializable
{
    private string? _name;

    public ResourceType Type { get; set; }

    public int ID { get; set; }

    public string Name
    {
        readonly get => _name ?? string.Empty;
        set => _name = value;
    }

    public void Read(BinaryObjectReader reader)
    {
        Type = (ResourceType)reader.ReadInt32();
        ID = reader.ReadInt32();
        Name = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)Type);
        writer.WriteInt32(ID);
        writer.WriteString(StringBinaryFormat.NullTerminated, Name);
        writer.Align(4);
    }

    public override string ToString()
    {
        return Name;
    }
}
