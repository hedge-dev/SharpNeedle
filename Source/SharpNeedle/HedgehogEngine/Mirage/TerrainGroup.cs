namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/terrain-group", @"\.terrain-group$")]
public class TerrainGroup : SampleChunkResource
{
    public List<string> ModelNames { get; set; }
    public List<SubSet> Sets { get; set; }

    public SubSet GetSubSet(Vector3 point)
    {
        foreach (var set in Sets)
            if (set.Bounds.Intersects(point)) return set;

        return null;
    }

    public override void Read(BinaryObjectReader reader)
    {
        Sets = reader.ReadObject<BinaryList<BinaryPointer<SubSet>>>().Unwind();
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
        writer.Write(Sets.Count);
        writer.WriteOffset(() =>
        {
            foreach (var set in Sets)
                writer.WriteObjectOffset(set);
        });

        writer.Write(ModelNames.Count);
        writer.WriteOffset(() =>
        {
            foreach (var name in ModelNames)
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, name);
        });
    }

    public class SubSet : List<string>, IBinarySerializable
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