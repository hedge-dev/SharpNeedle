namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/terrain-group", @"\.terrain-group$")]
public class TerrainGroup : SampleChunkResource
{
    public List<string> ModelNames { get; set; }
    public List<SubSet> Sets { get; set; }

    public (string Name, Sphere Bounds)? GetInstance(Vector3 point)
    {
        foreach (var set in Sets)
        {
            var instance = set.GetInstance(point);
            if (instance == null) continue;

            return instance;
        }
        return null;
    }

    public IEnumerable<(string Name, Sphere Bounds)> GetInstances(Vector3 point)
    {
        foreach (var set in Sets)
        {
            foreach (var instance in set.GetInstances(point))
                yield return instance;
        }
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

    public class SubSet : List<(string Name, Sphere Bounds)>, IBinarySerializable
    {
        public bool HasInstance(string name) => this.Any(x => x.Name == name);

        public (string Name, Sphere Bounds)? GetInstance(Vector3 point)
        {
            foreach (var instance in this)
            {
                if (instance.Bounds.Intersects(point))
                    return instance;
            }

            return null;
        }

        public IEnumerable<(string Name, Sphere Bounds)> GetInstances(Vector3 point)
        {
            foreach (var instance in this)
                if (instance.Bounds.Intersects(point))
                    yield return instance;
        }

        public void Read(BinaryObjectReader reader)
        {
            reader.Read(out int instanceCount);
            Clear();
            Capacity = instanceCount;

            reader.ReadOffset(() =>
            {
                for (int i = 0; i < instanceCount; i++)
                    Add(new (reader.ReadStringOffset(), default));
            });

            reader.ReadOffset(() =>
            {
                for (int i = 0; i < instanceCount; i++)
                {
                    var instance = this[i];
                    instance.Bounds = reader.Read<Sphere>();
                    this[i] = instance;
                }
            });
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Count);

            writer.WriteOffset(() =>
            {
                foreach (var instance in this)
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, instance.Name);
            });

            writer.WriteOffset(() =>
            {
                foreach (var instance in this)
                    writer.Write(instance.Bounds);
            });
        }
    }
}