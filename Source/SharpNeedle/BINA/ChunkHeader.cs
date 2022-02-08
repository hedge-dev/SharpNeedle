namespace SharpNeedle.BINA;

[StructLayout(LayoutKind.Sequential, Size = BinarySize)]
public struct ChunkHeader : IBinarySerializable
{
    public const int BinarySize = 8;
    public uint Signature;
    public uint Size;

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadNative<uint>();
        reader.Read(out Size);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteNative(Signature);
        writer.Write(ref Size);
    }
}