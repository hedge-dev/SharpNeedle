namespace SharpNeedle.Framework.HedgehogEngine.Mirage.TerrainData;
[NeedleResource("hh/terrain-group", @"\.terrain-group$")]
public class TerrainGroup : SampleChunkResource
{
    public List<string?> ModelNames { get; set; } = [];
    public List<TerrainGroupSubset> Subsets { get; set; } = [];

    public TerrainGroupSubset? GetSubset(Vector3 point)
    {
        foreach (TerrainGroupSubset set in Subsets)
        {
            if (set.Bounds.Intersects(point))
            {
                return set;
            }
        }

        return null;
    }

    public override void Read(BinaryObjectReader reader)
    {
        Subsets = reader.ReadObject<BinaryList<BinaryPointer<TerrainGroupSubset>>>().Unwind();
        reader.Read(out int modelCount);
        ModelNames = new(modelCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < modelCount; i++)
            {
                ModelNames.Add(reader.ReadStringOffset());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Subsets.Count);
        writer.WriteOffset(() =>
        {
            foreach (TerrainGroupSubset set in Subsets)
            {
                writer.WriteObjectOffset(set);
            }
        });

        writer.Write(ModelNames.Count);
        writer.WriteOffset(() =>
        {
            foreach (string? name in ModelNames)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, name);
            }
        });
    }

}