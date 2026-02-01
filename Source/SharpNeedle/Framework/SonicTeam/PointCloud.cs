namespace SharpNeedle.Framework.SonicTeam;

using SharpNeedle.Framework.BINA;

[NeedleResource("st/pointcloud", @"\.pc(model|col|rt)$")]
public class PointCloud : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("CPIC");
    public uint FormatVersion { get; set; } = 2; // Either 1 or 2, the data is the same regardless
    public List<InstanceData> Instances { get; set; } = [];

    public PointCloud()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        FormatVersion = reader.ReadUInt32();

        long instancesOffset = reader.ReadOffsetValue();
        long instanceCount = reader.ReadOffsetValue();

        reader.ReadAtOffset(instancesOffset, () =>
        {
            for (int i = 0; i < instanceCount; i++)
            {
                Instances.Add(reader.ReadObject<InstanceData, bool>(i == instanceCount - 1));
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Signature);
        writer.WriteUInt32(FormatVersion);

        writer.WriteOffset(() =>
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                writer.WriteObject(Instances[i], i == Instances.Count - 1);
            }
        });

        writer.WriteInt64(Instances.Count);
    }

    public class InstanceData : IBinarySerializable<bool>
    {
        public string? Name { get; set; }
        public string? ResourceName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int Field28 { get; set; }
        public Vector3 Scale { get; set; }

        public void Read(BinaryObjectReader reader, bool isLast = false)
        {
            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            ResourceName = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            Position = reader.ReadVector3();
            Rotation = reader.ReadVector3();
            Field28 = reader.ReadInt32();
            Scale = reader.ReadVector3();

            reader.Skip(4);

            // The last instance is never aligned
            if (!isLast)
            {
                reader.Align(8);
            }
        }

        public void Write(BinaryObjectWriter writer, bool isLast = false)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ResourceName, -1, 1);
            writer.WriteVector3(Position);
            writer.WriteVector3(Rotation);
            writer.WriteInt32(Field28);
            writer.WriteVector3(Scale);

            writer.Skip(4);

            // The last instance is never aligned
            if (!isLast)
            {
                writer.Align(8);
            }
        }
    }
}
