namespace SharpNeedle.Ninja;

[StructLayout(LayoutKind.Sequential, Size = BinarySize)]
public struct ChunkHeader
{
    public const int BinarySize = 8;
    public uint Signature;
    public int Size;
}