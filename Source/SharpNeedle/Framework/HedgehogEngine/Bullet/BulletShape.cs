namespace SharpNeedle.Framework.HedgehogEngine.Bullet;

public class BulletShape : IBinarySerializable<int>
{
    public BulletShapeFlags Flags { get; private set; }

    public bool IsConvex
    {
        get => Flags.HasFlag(BulletShapeFlags.IsConvexShape);
        set => Flags = value ? Flags | BulletShapeFlags.IsConvexShape : Flags & ~BulletShapeFlags.IsConvexShape;
    }

    public uint Layer { get; set; }


    public Vector3[] Vertices { get; set; } = [];

    public uint[]? Faces { get; set; } = [];

    public byte[]? BVH { get; set; } = [];


    public ulong[] Types { get; set; } = [];

    public uint Unknown1 { get; set; }

    public uint Unknown2 { get; set; }

    public void Read(BinaryObjectReader reader, int version)
    {
        Flags = (BulletShapeFlags)reader.ReadByte();
        reader.Align(4);

        if (version == 3)
        {
            Layer = reader.ReadUInt32();
        }

        uint vertexCount = reader.ReadUInt32();
        uint faceCount = reader.ReadUInt32();
        uint bvhSize = reader.ReadUInt32();
        uint typeCount = reader.ReadUInt32();

        Unknown1 = reader.ReadUInt32();

        if (version == 3)
        {
            Unknown2 = reader.ReadUInt32();
        }

        reader.ReadOffset(() => Vertices = reader.ReadVector3Array((int)vertexCount));

        if (IsConvex)
        {
            reader.ReadOffsetValue(); // skip faces
            reader.ReadOffsetValue(); // skip bvh
        }
        else
        {
            if (Flags.HasFlag(BulletShapeFlags.UShortIndexFormat))
            {
                Faces = new uint[faceCount * 3];

                reader.ReadOffset(() =>
                {
                    for (int i = 0; i < Faces.Length; i++)
                    {
                        Faces[i] = reader.ReadUInt16();
                    }
                });
            }
            else
            {
                Faces = reader.ReadArrayOffset<uint>((int)faceCount * 3);
            }

            BVH = reader.ReadArrayOffset<byte>((int)bvhSize);
        }

        if (version == 1)
        {
            Types = reader.ReadArrayOffset<ulong>((int)typeCount);
        }
        else
        {
            Types = new ulong[typeCount];

            reader.ReadOffset(() =>
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    Types[i] = reader.ReadUInt32();
                }
            });
        }
    }

    public void Write(BinaryObjectWriter writer, int version)
    {
        if (IsConvex || Faces!.Any(x => x > ushort.MaxValue))
        {
            Flags &= ~BulletShapeFlags.UShortIndexFormat;
        }
        else
        {
            Flags |= BulletShapeFlags.UShortIndexFormat;
        }

        writer.WriteByte((byte)Flags);
        writer.Align(4);

        if (version == 3)
        {
            writer.WriteUInt32(Layer);
        }

        writer.WriteUInt32((uint)Vertices.Length);
        writer.WriteUInt32(IsConvex ? 0 : (uint)Faces!.Length / 3);
        writer.WriteUInt32(IsConvex ? 0 : (uint)BVH!.Length);
        writer.WriteUInt32((uint)Types.Length);
        writer.WriteUInt32(Unknown1);

        if (version == 3)
        {
            writer.WriteUInt32(Unknown2);
        }

        writer.WriteOffset(() => writer.WriteVector3Array(Vertices), 0x10);

        if (IsConvex)
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
        }
        else
        {
            if (Flags.HasFlag(BulletShapeFlags.UShortIndexFormat))
            {
                writer.WriteOffset(() =>
                {
                    for (int i = 0; i < Faces!.Length; i++)
                    {
                        writer.WriteUInt16((ushort)Faces[i]);
                    }
                }, 0x10);
            }
            else
            {
                writer.WriteArrayOffset(Faces, 0x10);
            }

            writer.WriteArrayOffset(BVH, 0x10);
        }

        if (version == 1)
        {
            writer.WriteArrayOffset(Types, 0x10);
        }
        else
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Types!.Length; i++)
                {
                    writer.WriteUInt32((uint)Types[i]);
                }
            }, 0x10);
        }
    }
}
