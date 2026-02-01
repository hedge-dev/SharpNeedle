namespace SharpNeedle.Framework.BINA;

[StructLayout(LayoutKind.Sequential, Size = BinarySize)]
public struct ChunkHeader : IBinarySerializable
{
    public const int BinarySize = 8;
    public uint Signature;
    public uint Size;

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadNative<uint>();
        Size = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteNative(Signature);
        writer.WriteUInt32(Size);
    }
}