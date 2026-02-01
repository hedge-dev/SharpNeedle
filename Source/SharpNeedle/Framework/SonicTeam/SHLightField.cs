namespace SharpNeedle.Framework.SonicTeam;

using SharpNeedle.Framework.BINA;

[NeedleResource("st/shlf", @"\.(sh?)lf$")]
public class SHLightField : BinaryResource
{
    public uint FormatVersion { get; set; } = 1;
    public float[] DefaultProbeLightingData { get; set; } = new float[36];
    public List<Node> Nodes { get; set; } = [];

    public SHLightField()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        bool inMeters = Path.GetExtension(BaseFile?.Name) == ".lf";

        FormatVersion = reader.ReadUInt32();
        reader.ReadArray<float>(36, DefaultProbeLightingData);

        int probeCount = reader.ReadInt32();
        Nodes.AddRange(reader.ReadObjectArrayOffset<Node, bool>(inMeters, probeCount));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        bool inMeters = Path.GetExtension(BaseFile?.Name) == ".lf";

        writer.WriteUInt32(FormatVersion);
        writer.WriteArrayFixedLength(DefaultProbeLightingData, 36);

        writer.WriteInt32(Nodes.Count);
        writer.WriteObjectCollectionOffset(inMeters, Nodes);
    }

    public class Node : IBinarySerializable<bool>
    {
        public string? Name { get; set; }

        public int ProbeCountX { get; set; }
        public int ProbeCountY { get; set; }
        public int ProbeCountZ { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public void Read(BinaryObjectReader reader, bool inMeters = false)
        {
            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            ProbeCountX = reader.ReadInt32();
            ProbeCountY = reader.ReadInt32();
            ProbeCountZ = reader.ReadInt32();

            Position = inMeters ? reader.ReadVector3() : reader.ReadVector3() / 10;
            Rotation = reader.ReadVector3();
            Scale = inMeters ? reader.ReadVector3() : reader.ReadVector3() / 10;
        }

        public void Write(BinaryObjectWriter writer, bool inMeters = false)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, -1, 1);

            writer.WriteInt32(ProbeCountX);
            writer.WriteInt32(ProbeCountY);
            writer.WriteInt32(ProbeCountZ);

            writer.WriteVector3(inMeters ? Position : Position * 10);
            writer.WriteVector3(Rotation);
            writer.WriteVector3(inMeters ? Scale : Scale * 10);
        }
    }
}
