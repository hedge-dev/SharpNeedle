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
        FormatVersion = reader.ReadUInt32();

        long instancesOffset = reader.ReadOffsetValue();
        int instanceCount = (int)reader.ReadInt64();

        reader.ReadAtOffset(instancesOffset, () => Layers.AddRange(reader.ReadObjectArray<TerrainLayer>(instanceCount)));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Signature);
        writer.WriteUInt32(FormatVersion);

        writer.WriteObjectCollectionOffset(Layers);
        writer.WriteInt64(Layers.Count);
    }

    public class TerrainLayer : IBinarySerializable
    {
        public static readonly string[] Types = { "grass", "gravel", "hole", "bare", "earth", "rock", "flowerground" };
        public string? Type { get; set; }
        public int SplatIndex { get; set; }
        public int Field0C { get; set; }
        public int Field10 { get; set; }
        public int Field14 { get; set; }
        public string? DetailAlbedoMap { get; set; }
        public string? DetailNormalMap { get; set; }
        public string? DetailHeightMap { get; set; }
        public string? BaseAlbedoMap { get; set; }
        public string? BaseNormalMap { get; set; }
        public string? BaseParameterMap { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Type = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

            SplatIndex = reader.ReadInt32();
            Field0C = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            Field14 = reader.ReadInt32();

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

            writer.WriteInt32(SplatIndex);
            writer.WriteInt32(Field0C);
            writer.WriteInt32(Field10);
            writer.WriteInt32(Field14);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, DetailAlbedoMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, DetailNormalMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, DetailHeightMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, BaseAlbedoMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, BaseNormalMap, -1, 1);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, BaseParameterMap, -1, 1);
        }
    }
}
