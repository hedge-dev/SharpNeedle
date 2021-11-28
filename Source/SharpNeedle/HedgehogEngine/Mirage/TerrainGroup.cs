namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/terrain-group", @"\.terrain-group$")]
public class TerrainGroup : SampleChunkResource
{
    public List<string> ModelNames { get; set; }
    public List<SubSet> Sets { get; set; }

    public string GetInstance(Vector3 point)
    {
        foreach (var set in Sets)
        {
            var name = set.GetInstance(point);
            if (name == null) continue;

            return name;
        }
        return null;
    }

    public IEnumerable<string> GetInstances(Vector3 point)
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

    public class SubSet : IBinarySerializable
    {
        public Dictionary<string, Sphere> Instances { get; set; } = new(2);

        public bool HasInstance(string name) => Instances.ContainsKey(name);

        public string GetInstance(Vector3 point)
        {
            foreach (var instance in Instances)
            {
                if (instance.Value.Intersects(point))
                    return instance.Key;
            }

            return null;
        }

        public IEnumerable<string> GetInstances(Vector3 point)
        {
            foreach (var instance in Instances)
                if (instance.Value.Intersects(point))
                    yield return instance.Key;
        }

        public void Read(BinaryObjectReader reader)
        {
            var instances = new(string Name, Sphere Range)[reader.Read<int>()];
            reader.ReadOffset(() =>
            {
                for (int i = 0; i < instances.Length; i++)
                    instances[i].Name = reader.ReadStringOffset();
            });

            reader.ReadOffset(() =>
            {
                for (int i = 0; i < instances.Length; i++)
                    instances[i].Range = reader.Read<Sphere>();
            });

            foreach (var instance in instances)
                Instances.Add(instance.Name, instance.Range);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Instances.Count);

            writer.WriteOffset(() =>
            {
                foreach (var instance in Instances)
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, instance.Key);
            });

            writer.WriteOffset(() =>
            {
                foreach (var instance in Instances)
                    writer.Write(instance.Value);
            });
        }
    }
}