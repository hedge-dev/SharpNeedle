namespace SharpNeedle.SurfRide;

[StructLayout(LayoutKind.Sequential, Size = BinarySize)]
public struct ChunkHeader : IBinarySerializable
{
    public const int BinarySize = 8;
    public uint Signature;
    public int Size;

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadLittle<uint>();
        reader.Read(out Size);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteLittle(Signature);
        writer.Write(Size);
    }
}