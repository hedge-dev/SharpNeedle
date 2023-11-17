namespace SharpNeedle.SonicTeam;

using System.IO;
using BINA;
using Utilities;

[NeedleResource("st/shlf", @"\.(sh?)lf$")]
public class SHLightField : BinaryResource
{
    public uint FormatVersion { get; set; } = 1;
    public float[] DefaultProbeLightingData { get; set; } = new float[36];
    public List<Node> Nodes { get; set; } = new ();

    public SHLightField()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        bool inMeters = Path.GetExtension(BaseFile.Name) == ".lf";

        FormatVersion = reader.Read<uint>();
        reader.ReadArray<float>(36, DefaultProbeLightingData);

        int probeCount = reader.Read<int>();
        Nodes.AddRange(reader.ReadObjectArrayOffset<Node, bool>(inMeters, probeCount));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        bool inMeters = Path.GetExtension(BaseFile.Name) == ".lf";

        writer.Write(FormatVersion);
        writer.WriteArrayFixedLength(DefaultProbeLightingData, 36);

        writer.Write(Nodes.Count);
        writer.WriteObjectCollectionOffset(inMeters, Nodes);
    }

    public class Node : IBinarySerializable<bool>
    {
        public string Name { get; set; }

        public int ProbeCountX { get; set; }
        public int ProbeCountY { get; set; }
        public int ProbeCountZ { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public void Read(BinaryObjectReader reader, bool inMeters = false)
        {
            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            ProbeCountX = reader.Read<int>();
            ProbeCountY = reader.Read<int>();
            ProbeCountZ = reader.Read<int>();

            Position = inMeters ? reader.Read<Vector3>() : reader.Read<Vector3>() / 10;
            Rotation = reader.Read<Vector3>();
            Scale = inMeters ? reader.Read<Vector3>() : reader.Read<Vector3>() / 10;
        }

        public void Write(BinaryObjectWriter writer, bool inMeters = false)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name, -1, 1);

            writer.Write(ProbeCountX);
            writer.Write(ProbeCountY);
            writer.Write(ProbeCountZ);

            writer.Write(inMeters ? Position : Position * 10);
            writer.Write(Rotation);
            writer.Write(inMeters ? Scale : Scale * 10);
        }
    }
}
