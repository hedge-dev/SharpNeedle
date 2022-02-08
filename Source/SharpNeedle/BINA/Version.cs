namespace SharpNeedle.BINA;

public struct Version : IBinarySerializable
{
    public byte Major;
    public byte Minor;
    public byte Revision;
    public Endianness Endianness;

    public void Read(BinaryObjectReader reader)
    {
        var ma = reader.Read<byte>();
        var mi = reader.Read<byte>();
        var r = reader.Read<byte>();

        Major = ma < 0x30 ? ma : (byte)(ma - 0x30);
        Minor = mi < 0x30 ? mi : (byte)(mi - 0x30);
        Revision = r < 0x30 ? r : (byte)(r - 0x30);

        var e = reader.Read<byte>();
        switch (e)
        {
            case 0x4C:
                Endianness = Endianness.Little;
                break;

            case 0x42:
                Endianness = Endianness.Big;
                break;
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        if (Major == 0)
        {
            writer.Write(Major);
            writer.Write(Minor);
            writer.Write((byte)(Revision - 0x30));
        }
        else
        {
            writer.Write((byte)(Major + 0x30));
            writer.Write((byte)(Minor + 0x30));
            writer.Write((byte)(Revision + 0x30));
        }

        switch (Endianness)
        {
            case Endianness.Little:
                writer.Write((byte)0x4C);
                break;

            case Endianness.Big:
                writer.Write((byte)0x42);
                break;
        }
    }
}