namespace SharpNeedle.Framework.SurfRide;

[StructLayout(LayoutKind.Sequential, Size = BinarySize)]
public struct ChunkHeader : IBinarySerializable
{
    public const int BinarySize = 8;
    public uint Signature;
    public int Size;

    public void Read(BinaryObjectReader reader)
    {
        Signature = reader.ReadLittle<uint>();
        Size = reader.ReadInt32();
    }

    public readonly void Write(BinaryObjectWriter writer)
    {
        writer.WriteLittle(Signature);
        writer.WriteInt32(Size);
    }
}