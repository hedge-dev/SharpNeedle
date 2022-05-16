namespace SharpNeedle.Ninja.Csd;
using Motions;
using FrameInfo = ValueTuple<float, float>;

public class Scene : IBinarySerializable
{
    public int Version { get; set; } = 3;
    public float Priority { get; set; }
    public float FrameRate { get; set; }
    public float AspectRatio { get; set; } = 1.333333F; // Default Aspect Ratio is 4:3
    public List<Vector2> Textures { get; set; } // Size of textures used in the scene
    public List<Sprite> Sprites { get; set; }
    public List<Family> Families { get; set; }
    public CsdDictionary<Motion> Motions { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Version = reader.Read<int>();
        Priority = reader.Read<float>();
        FrameRate = reader.Read<float>();
        var motionBegin = reader.Read<float>();
        var motionEnd = reader.Read<float>();
        Textures = new List<Vector2>(reader.ReadArrayOffset<Vector2>(reader.Read<int>()));
        Sprites = new List<Sprite>(reader.ReadArrayOffset<Sprite>(reader.Read<int>()));

        var familyCount = reader.Read<int>();
        Families = new List<Family>(familyCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < familyCount; i++)
                Families.Add(reader.ReadObject<Family, Scene>(this));
        });
        
        var castInfo = reader.ReadObject<CastInfoTable>();
        Motions = reader.ReadObject<CsdDictionary<Motion>>();

        foreach (var motion in Motions)
            motion.Value.Attach(this);

        for (int i = 0; i < Motions.Count; i++)
        {
            Motions[i].StartFrame = motionBegin;
            Motions[i].EndFrame = motionEnd;
        }

        if (Version >= 1)
            AspectRatio = reader.Read<float>();

        if (Version >= 2)
        {
            var frameInfo = reader.ReadArrayOffset<FrameInfo>(Motions.Count);
            for (int i = 0; i < Motions.Count; i++)
            {
                Motions[i].StartFrame = frameInfo[i].Item1;
                Motions[i].EndFrame = frameInfo[i].Item2;
            }
        }

        if (Version >= 3)
        {
            reader.ReadOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                    Motions[i].ReadExtended(reader);
            });
        }

        foreach (var item in castInfo)
            Families[item.FamilyIdx].Casts[item.CastIdx].Name = item.Name;
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Version);
        writer.Write(Priority);
        writer.Write(FrameRate);
        writer.Write(Motions.Count != 0 ? Motions.Min(x => x.Value.StartFrame) : 0);
        writer.Write(Motions.Count != 0 ? Motions.Max(x => x.Value.EndFrame) : 0);

        writer.Write(Textures.Count);
        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Textures).AsMemory());

        writer.Write(Sprites.Count);
        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Sprites).AsMemory());

        writer.Write(Families.Count);
        writer.WriteOffset(() =>
        {
            foreach (var family in Families)
                writer.WriteObject(family);
        });
        
        writer.WriteObject(BuildCastTable());
        writer.WriteObject(Motions);

        if (Version >= 1)
            writer.Write(AspectRatio);

        if (Version >= 2)
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                    writer.Write(new FrameInfo(Motions[i].StartFrame, Motions[i].EndFrame));
            });
        }

        if (Version >= 3)
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                    Motions[i].WriteExtended(writer);
            });
        }
    }

    public CastInfoTable BuildCastTable()
    {
        var result = new CastInfoTable();
        for (int i = 0; i < Families.Count; i++)
        {
            var family = Families[i];
            for (int c = 0; c < family.Casts.Count; c++)
            {
                result.Add(new (family.Casts[c].Name, i, c));
            }
        }

        // Sort because game uses binary search to look for casts
        result.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        return result;
    }
}