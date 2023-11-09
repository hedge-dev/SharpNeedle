namespace SharpNeedle.SonicTeam;
using BINA;

[NeedleResource("st/pointcloud", @"\.pc(model|col|rt)$")]
public class PointCloud : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("CPIC");
    public uint FormatVersion { get; set; } = 2; // Either 1 or 2, the data is the same regardless
    public List<InstanceData> Instances { get; set; } = new();

    public PointCloud()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        FormatVersion = reader.Read<uint>();

        var instancesOffset = reader.ReadOffsetValue();
        var instanceCount = reader.ReadOffsetValue();

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
        writer.Write(Signature);
        writer.Write(FormatVersion);

        writer.WriteOffset(() =>
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                writer.WriteObject(Instances[i], i == Instances.Count - 1);
            }
        });

        writer.Write<long>(Instances.Count);
    }

    public class InstanceData : IBinarySerializable<bool>
    {
        public string Name { get; set; }
        public string ResourceName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int Field28 { get; set; }
        public Vector3 Scale { get; set; }

        public void Read(BinaryObjectReader reader, bool isLast = false)
        {
            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            ResourceName = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            Position = reader.Read<Vector3>();
            Rotation = reader.Read<Vector3>();
            Field28 = reader.Read<int>();
            Scale = reader.Read<Vector3>();

            reader.Skip(4);

            // The last instance is never aligned
            if (!isLast)
                reader.Align(8);
        }

        public void Write(BinaryObjectWriter writer, bool isLast = false)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ResourceName, -1, 1);
            writer.Write(Position);
            writer.Write(Rotation);
            writer.Write(Field28);
            writer.Write(Scale);

            writer.Skip(4);

            // The last instance is never aligned
            if (!isLast)
                writer.Align(8); 
        }
    }
}
