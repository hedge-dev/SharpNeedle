namespace SharpNeedle.Framework.LostWorld.Animation;

using SharpNeedle.Utilities;

public class AnimationDef : IBinarySerializable
{
    public string? Name { get; set; }
    public short Layer { get; set; }
    public AnimationType Type { get; }

    public AnimationDef(AnimationType type)
    {
        Type = type;
    }

    public virtual void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        reader.EnsureSignatureNative(Type);
        Layer = reader.Read<short>();
    }

    public virtual void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Type);
        writer.Write(Layer);
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}

public enum AnimationType : short
{
    Simple, Complex
}