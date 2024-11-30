namespace SharpNeedle.HedgehogEngine.Mirage;

[NeedleResource("hh/terrain", @"\.terrain$")]
public class Terrain : SampleChunkResource
{
    public List<GroupInfo> Groups { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        Groups = reader.ReadObject<BinaryList<BinaryPointer<GroupInfo>>>().Unwind();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Groups.Count);
        writer.WriteOffset(() =>
        {
            foreach(GroupInfo group in Groups)
            {
                writer.WriteObjectOffset(group);
            }
        });
    }

    public class GroupInfo : IBinarySerializable
    {
        public string Name { get; set; }
        public Sphere Bounds { get; set; }
        public uint MemorySize { get; set; }
        public int SubsetID { get; set; }
        public List<Sphere> Instances { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Bounds = reader.ReadValueOffset<Sphere>();
            Name = reader.ReadStringOffset();
            MemorySize = reader.Read<uint>();

            reader.Read(out int instancesCount);
            Instances = new List<Sphere>(instancesCount);
            reader.ReadOffset(() =>
            {
                for(int i = 0; i < instancesCount; i++)
                {
                    Instances.Add(reader.ReadValueOffset<Sphere>());
                }
            });

            SubsetID = reader.Read<int>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteValueOffset(Bounds);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(MemorySize);
            writer.Write(Instances.Count);
            writer.WriteOffset(() =>
            {
                foreach(Sphere instance in Instances)
                {
                    writer.WriteValueOffset(instance);
                }
            });
        }

        public override string ToString()
        {
            return $"{Name}:{SubsetID}";
        }
    }
}