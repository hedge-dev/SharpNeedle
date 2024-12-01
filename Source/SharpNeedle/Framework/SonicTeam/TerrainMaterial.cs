namespace SharpNeedle.Framework.SonicTeam;

using SharpNeedle.Framework.BINA;

[NeedleResource("st/terrain-material", @"\.terrain-material$")]
public class TerrainMaterial : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("MTDN");
    public uint FormatVersion { get; set; } = 1;
    public List<TerrainLayer> Layers { get; set; } = [];

    public TerrainMaterial()
    {
        Version = new Version(2, 1, 0, BinaryHelper.PlatformEndianness);
    }

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        FormatVersion = reader.Read<uint>();

        long instancesOffset = reader.ReadOffsetValue();
        int instanceCount = (int)reader.Read<long>();

        reader.ReadAtOffset(instancesOffset, () => Layers.AddRange(reader.ReadObjectArray<TerrainLayer>(instanceCount)));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Signature);
        writer.Write(FormatVersion);

        writer.WriteObjectCollectionOffset(Layers);
        writer.Write<long>(Layers.Count);
    }

    public class TerrainLayer : IBinarySerializable
    {
        public static readonly string[] Types = { "grass", "gravel", "hole", "bare", "earth", "rock", "flowerground" };
        public string Type { get; set; }
        public int SplatIndex { get; set; }
        public int Field0C { get; set; }
        public int Field10 { get; set; }
        public int Field14 { get; set; }
        public string DetailAlbedoMap { get; set; }
        public string DetailNormalMap { get; set; }
        public string DetailHeightMap { get; set; }
        public string BaseAlbedoMap { get; set; }
        public string BaseNormalMap { get; set; }
        public string BaseParameterMap { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Type = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            SplatIndex = reader.Read<int>();
            Field0C = reader.Read<int>();
            Field10 = reader.Read<int>();
            Field14 = reader.Read<int>();

            DetailAlbedoMap = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            DetailNormalMap = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            DetailHeightMap = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            BaseAlbedoMap = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            BaseNormalMap = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            BaseParameterMap = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Type, -1, 1);

            writer.Write(SplatIndex);
            writer.Write(Field0C);
            writer.Write(Field10);
            writer.Write(Field14);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, DetailAlbedoMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, DetailNormalMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, DetailHeightMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, BaseAlbedoMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, BaseNormalMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, BaseParameterMap, -1, 1);
        }
    }
}
