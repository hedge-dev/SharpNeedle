namespace SharpNeedle.Framework.HedgehogEngine.Mirage.TerrainData;
[NeedleResource("hh/terrain", @"\.terrain$")]
public class Terrain : SampleChunkResource
{
    public List<TerrainGroupInfo> Groups { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        Groups = reader.ReadObject<BinaryList<BinaryPointer<TerrainGroupInfo>>>().Unwind();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Groups.Count);
        writer.WriteOffset(() =>
        {
            foreach (TerrainGroupInfo group in Groups)
            {
                writer.WriteObjectOffset(group);
            }
        });
    }
}