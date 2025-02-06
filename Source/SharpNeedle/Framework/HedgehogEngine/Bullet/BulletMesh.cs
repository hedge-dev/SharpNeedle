namespace SharpNeedle.Framework.HedgehogEngine.Bullet;

using Amicitia.IO.Binary;
using SharpNeedle.Framework.BINA;

[NeedleResource("hh/btmesh", ResourceType.Model, @"\.btmesh$")]
public class BulletMesh : BinaryResource
{
    public int BulletMeshVersion { get; set; } = 3;

    public BulletShape[] Shapes { get; set; } = [];

    public BulletPrimitive[] Primitives { get; set; } = [];

    public BulletMesh()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        reader.Skip(4);
        int version = reader.ReadInt32();

        if (version is 0 or 2)
        {
            throw new InvalidDataException($"Unknown bullet mesh version ({version})");
        }

        BulletMeshVersion = version;
        long shapesOffset = reader.ReadOffsetValue();
        Shapes = new BulletShape[reader.ReadInt32()];

        reader.ReadAtOffset(shapesOffset, () =>
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                Shapes[i] = reader.ReadObject<BulletShape, int>(BulletMeshVersion);
            }
        });

        int primitiveCount = reader.ReadInt32();
        Primitives = reader.ReadObjectArrayOffset<BulletPrimitive>(primitiveCount);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteNulls(4);
        writer.WriteInt32(BulletMeshVersion);

        if (Shapes.Length == 0)
        {
            writer.WriteOffsetValue(0);
        }
        else
        {
            writer.WriteOffset(() =>
            {
                foreach (BulletShape shape in Shapes)
                {
                    writer.WriteObject(shape, BulletMeshVersion);
                }
            });
        }

        writer.WriteInt32(Shapes.Length);

        writer.WriteInt32(Primitives.Length);
        if (Primitives.Length == 0)
        {
            writer.WriteOffsetValue(0);
        }
        else
        {
            writer.WriteObjectCollectionOffset(Primitives);
        }
    }
}
