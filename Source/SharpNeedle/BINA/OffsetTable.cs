namespace SharpNeedle.BINA;
using System.IO;

public class OffsetTable : List<long>
{
    public byte[] Encode()
    {
        return Encode(this);
    }

    public void Encode(Stream stream)
    {
        Encode(this, stream);
    }

    public void Encode(BinaryValueWriter writer)
    {
        Encode(this, writer);
    }

    public void Decode(byte[] table)
    {
        Clear();
        Decode(table, this);
    }

    public static OffsetTable FromBytes(byte[] table)
    {
        OffsetTable result = [];
        Decode(table, result);

        return result;
    }

    public static void Decode(byte[] table, List<long> outOffsets)
    {
        long lastOffset = 0L;

        for(int i = 0; i < table.Length; i++)
        {
            byte b = table[i];
            OffsetEncoding type = (OffsetEncoding)(b & 0xC0);
            byte v = (byte)(b & 0x3F);

            switch(type)
            {
                case OffsetEncoding.SixBit:
                    outOffsets.Add((v << 2) + lastOffset);
                    break;

                case OffsetEncoding.FourteenBit:
                {
                    byte v2 = table[++i];
                    outOffsets.Add(((ushort)(((v << 8) | v2) << 2)) + lastOffset);
                    break;
                }

                case OffsetEncoding.ThirtyBit:
                {
                    byte v2 = table[++i];
                    byte v3 = table[++i];
                    byte v4 = table[++i];
                    outOffsets.Add(((uint)(((v << 24) | (v2 << 16) |
                                            (v3 << 8) | v4) << 2)) + lastOffset);
                    break;
                }
            }

            lastOffset = outOffsets[^1];
        }
    }

    public static byte[] Encode(IEnumerable<long> offsets)
    {
        using MemoryStream stream = new();
        Encode(offsets, stream);
        return stream.ToArray();
    }

    public static void Encode(IEnumerable<long> offsets, Stream outStream)
    {
        BinaryValueWriter writer = new(outStream, StreamOwnership.Retain, Endianness.Big);
        Encode(offsets, writer);
    }

    public static void Encode(IEnumerable<long> offsets, BinaryValueWriter writer)
    {
        long lastOffset = 0L;
        foreach(long offset in offsets)
        {
            long d = (offset - lastOffset) >> 2;
            if(d > 0x3FFF)
            {
                writer.WriteBig((byte)(0xC0 | (d >> 24)));
                writer.WriteBig((byte)(d >> 16));
                writer.WriteBig((byte)(d >> 8));
                writer.WriteBig((byte)(d & 0xFF));
            }
            else if(d > 0x3F)
            {
                writer.WriteBig((byte)(0x80 | (d >> 8)));
                writer.WriteBig((byte)d);
            }
            else
            {
                writer.Write((byte)(0x40 | d));
            }

            lastOffset = offset;
        }

        writer.Align(4);
    }
}

public enum OffsetEncoding
{
    SixBit = 0x40,
    FourteenBit = 0x80,
    ThirtyBit = 0xC0
}