namespace SharpNeedle.Framework.Ninja.Csd;

public struct Sprite : IBinarySerializable
{
    public int TextureIndex;
    public Vector2 TopLeft;
    public Vector2 BottomRight;

    public Sprite()
    {
        TopLeft = default;
        TextureIndex = default;
        BottomRight = new(1, 1);
    }

    public void Read(BinaryObjectReader reader)
    {
        TextureIndex = reader.ReadInt32();
        TopLeft = reader.ReadVector2();
        BottomRight = reader.ReadVector2();

    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(TextureIndex);
        writer.WriteVector2(TopLeft);
        writer.WriteVector2(BottomRight);
    }
}