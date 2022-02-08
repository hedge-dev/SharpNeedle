namespace SharpNeedle.HedgehogEngine.Mirage;

[NeedleResource("hh/terrain-group", @"\.terrain-group$")]
public class TerrainGroup : SampleChunkResource
{
    public List<string> ModelNames { get; set; }
    public List<Subset> Subsets { get; set; }

    public Subset GetSubset(Vector3 point)
    {
        foreach (var set in Subsets)
            if (set.Bounds.Intersects(point)) return set;

        return null;
    }

    public override void Read(BinaryObjectReader reader)
    {
        Subsets = reader.ReadObject<BinaryList<BinaryPointer<Subset>>>().Unwind();
        reader.Read(out int modelCount);
        ModelNames = new List<string>(modelCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < modelCount; i++)
                ModelNames.Add(reader.ReadStringOffset());
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Subsets.Count);
        writer.WriteOffset(() =>
        {
            foreach (var set in Subsets)
                writer.WriteObjectOffset(set);
        });

        writer.Write(ModelNames.Count);
        writer.WriteOffset(() =>
        {
            foreach (var name in ModelNames)
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, name);
        });
    }

    public class Subset : List<string>, IBinarySerializable
    {
        public Sphere Bounds { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            reader.Read(out int instanceCount);
            Clear();
            Capacity = instanceCount;

            reader.ReadOffset(() =>
            {
                for (int i = 0; i < instanceCount; i++)
                    Add(reader.ReadStringOffset());
            });

            Bounds = reader.ReadValueOffset<Sphere>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Count);

            writer.WriteOffset(() =>
            {
                foreach (var instance in this)
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, instance);
            });
            writer.WriteValueOffset(Bounds);
        }
    }
}