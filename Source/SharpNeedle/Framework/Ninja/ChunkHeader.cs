namespace SharpNeedle.Framework.Ninja;

[StructLayout(LayoutKind.Sequential, Size = BinarySize)]
public struct ChunkHeader : IBinarySerializable
{
    public const int BinarySize = 8;
    public uint Signature;
    public int Size;

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadLittle<uint>();
        Size = reader.ReadLittle<int>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteLittle(Signature);
        writer.WriteLittle(Size);
    }
}