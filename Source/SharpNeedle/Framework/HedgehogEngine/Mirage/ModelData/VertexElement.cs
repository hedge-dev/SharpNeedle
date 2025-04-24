namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct VertexElement
{
    public static readonly VertexElement Invalid = new() { Stream = 0xFF, Format = VertexFormat.Invalid };

    public ushort Stream;
    public ushort Offset;
    public VertexFormat Format;
    public VertexMethod Method;
    public VertexType Type;
    public byte UsageIndex;

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
                            Swap<float>();
                            break;
                        case VertexFormat.Float2:
                            Swap<Vector2>();
                            break;
                        case VertexFormat.Float3:
                            Swap<Vector3>();
                            break;
                        case VertexFormat.Float4:
                            Swap<Vector4>();
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
                            Swap<uint>();
                            break;

                        case VertexFormat.Int2:
                        case VertexFormat.Int2Norm:
                        case VertexFormat.Uint2:
                        case VertexFormat.Uint2Norm:
                            Swap<Vector2Int>();
                            break;

                        case VertexFormat.Int4:
                        case VertexFormat.Uint4:
                        case VertexFormat.Int4Norm:
                        case VertexFormat.Uint4Norm:
                            Swap<Vector4Int>();
                            break;

                        case VertexFormat.Short2:
                        case VertexFormat.Ushort2:
                        case VertexFormat.Short2Norm:
                        case VertexFormat.Ushort2Norm:
                        case VertexFormat.Float16_2:
                            Swap<ushort>();
                            pData += sizeof(ushort);
                            Swap<ushort>();
                            break;

                        case VertexFormat.Short4:
                        case VertexFormat.Ushort4:
                        case VertexFormat.Ushort4Norm:
                        case VertexFormat.Short4Norm:
                        case VertexFormat.Float16_4:
                            Swap<ushort>();
                            pData += sizeof(ushort);
                            Swap<ushort>();
                            pData += sizeof(ushort);
                            Swap<ushort>();
                            pData += sizeof(ushort);
                            Swap<ushort>();
                            break;

                        case VertexFormat.Invalid:
                            continue;

                        default:
                            throw new Exception($"Invalid Vertex Format {element.Format}");
                    }

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    void Swap<T>() where T : unmanaged
                    {
                        BinaryOperations<T>.Reverse(ref Unsafe.AsRef<T>(pData));
                    }
                }

                pVertex += size;
            }
        }
    }
}