namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

[StructLayout(LayoutKind.Sequential)]
public struct VertexElement : IBinarySerializable
{
    public static readonly VertexElement Invalid = new() { Stream = 0xFF, Format = VertexFormat.Invalid };

    public ushort Stream;
    public ushort Offset;
    public VertexFormat Format;
    public VertexMethod Method;
    public VertexType Type;
    public byte UsageIndex;

    public void Read(BinaryObjectReader reader)
    {
        Stream = reader.ReadUInt16();
        Offset = reader.ReadUInt16();
        Format = (VertexFormat)reader.ReadUInt32();
        Method = (VertexMethod)reader.ReadByte();
        Type = (VertexType)reader.ReadByte();
        UsageIndex = reader.ReadByte();
        reader.ReadByte();
    }

    public readonly void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt16(Stream);
        writer.WriteUInt16(Offset);
        writer.WriteUInt32((uint)Format);
        writer.WriteByte((byte)Method);
        writer.WriteByte((byte)Type);
        writer.WriteByte(UsageIndex);
        writer.WriteByte(0);
    }

    public static unsafe void SwapEndianness(VertexElement[] elements, Span<byte> vertices, nint count, nint size)
    {
        // I'm going to have a breakdown
        fixed (byte* pBegin = vertices)
        {
            byte* pVertex = pBegin;
            for (nint i = 0; i < count; i++)
            {
                foreach (VertexElement element in elements)
                {
                    byte* pData = pVertex + element.Offset;
                    switch (element.Format)
                    {
                        case VertexFormat.Float1:
                            Swap<float>(0);
                            break;
                        case VertexFormat.Float2:
                            Swap<float>(0);
                            Swap<float>(1);
                            break;
                        case VertexFormat.Float3:
                            Swap<float>(0);
                            Swap<float>(1);
                            Swap<float>(2);
                            break;
                        case VertexFormat.Float4:
                            Swap<float>(0);
                            Swap<float>(1);
                            Swap<float>(2);
                            Swap<float>(3);
                            break;

                        case VertexFormat.Byte4:
                        case VertexFormat.UByte4:
                        case VertexFormat.Byte4Norm:
                        case VertexFormat.UByte4Norm:
                        case VertexFormat.Int1:
                        case VertexFormat.Int1Norm:
                        case VertexFormat.Uint1:
                        case VertexFormat.Uint1Norm:
                        case VertexFormat.D3dColor:
                        case VertexFormat.UDec3:
                        case VertexFormat.Dec3:
                        case VertexFormat.UDec3Norm:
                        case VertexFormat.Dec3Norm:
                        case VertexFormat.UDec4:
                        case VertexFormat.Dec4:
                        case VertexFormat.Dec4Norm:
                        case VertexFormat.UDec4Norm:
                        case VertexFormat.UHend3:
                        case VertexFormat.Hend3:
                        case VertexFormat.Uhend3Norm:
                        case VertexFormat.Hend3Norm:
                        case VertexFormat.Udhen3:
                        case VertexFormat.Dhen3:
                        case VertexFormat.Dhen3Norm:
                        case VertexFormat.Udhen3Norm:
                            Swap<uint>(0);
                            break;

                        case VertexFormat.Int2:
                        case VertexFormat.Int2Norm:
                        case VertexFormat.Uint2:
                        case VertexFormat.Uint2Norm:
                            Swap<uint>(0);
                            Swap<uint>(1);
                            break;

                        case VertexFormat.Int4:
                        case VertexFormat.Uint4:
                        case VertexFormat.Int4Norm:
                        case VertexFormat.Uint4Norm:
                            Swap<uint>(0);
                            Swap<uint>(1);
                            Swap<uint>(2);
                            Swap<uint>(3);
                            break;

                        case VertexFormat.Short2:
                        case VertexFormat.Ushort2:
                        case VertexFormat.Short2Norm:
                        case VertexFormat.Ushort2Norm:
                        case VertexFormat.Float16_2:
                            Swap<ushort>(0);
                            Swap<ushort>(1);
                            break;

                        case VertexFormat.Short4:
                        case VertexFormat.Ushort4:
                        case VertexFormat.Ushort4Norm:
                        case VertexFormat.Short4Norm:
                        case VertexFormat.Float16_4:
                            Swap<ushort>(0);
                            Swap<ushort>(1);
                            Swap<ushort>(2);
                            Swap<ushort>(3);
                            break;

                        case VertexFormat.Invalid:
                            continue;

                        default:
                            throw new Exception($"Invalid Vertex Format {element.Format}");
                    }

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    void Swap<T>(int index) where T : unmanaged
                    {
                        BinaryOperations<T>.Reverse(ref Unsafe.AsRef<T>(((T*)pData) + index));
                    }
                }

                pVertex += size;
            }
        }
    }


}