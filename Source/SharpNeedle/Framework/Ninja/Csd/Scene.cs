namespace SharpNeedle.Framework.Ninja.Csd;

using SharpNeedle.Framework.Ninja.Csd.Motions;

public class Scene : IBinarySerializable
{
    public int Version { get; set; } = 3;
    public float Priority { get; set; }
    public float FrameRate { get; set; }
    public float AspectRatio { get; set; } = 1.333333F; // Default Aspect Ratio is 4:3
    public List<Vector2> Textures { get; set; } = []; // Size of textures used in the scene
    public List<Sprite> Sprites { get; set; } = [];
    public List<Family> Families { get; set; } = [];
    public CsdDictionary<Motion> Motions { get; set; } = [];

    public void Read(BinaryObjectReader reader)
    {
        Version = reader.ReadInt32();
        Priority = reader.ReadSingle();
        FrameRate = reader.ReadSingle();
        float motionBegin = reader.ReadSingle();
        float motionEnd = reader.ReadSingle();

        int texturesCount = reader.ReadInt32();
        reader.ReadOffset(() => Textures = [.. reader.ReadVector2Array(texturesCount)]);

        Sprites = [.. reader.ReadObjectArrayOffset<Sprite>(reader.ReadInt32())];

        int familyCount = reader.ReadInt32();
        Families = new List<Family>(familyCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < familyCount; i++)
            {
                Families.Add(reader.ReadObject<Family, Scene>(this));
            }
        });

        CastInfoTable castInfo = reader.ReadObject<CastInfoTable>();
        Motions = reader.ReadObject<CsdDictionary<Motion>>();

        foreach (KeyValuePair<string?, Motion> motion in Motions)
        {
            motion.Value.Attach(this);
        }

        for (int i = 0; i < Motions.Count; i++)
        {
            Motions[i].StartFrame = motionBegin;
            Motions[i].EndFrame = motionEnd;
        }

        if (Version >= 1)
        {
            AspectRatio = reader.ReadSingle();
        }

        if (Version >= 2)
        {
            reader.ReadOffset(() =>
            {
                for(int i = 0; i < Motions.Count; i++)
                {
                    Motions[i].StartFrame = reader.ReadSingle();
                    Motions[i].EndFrame = reader.ReadSingle();
                }
            });
        }

        if (Version >= 3)
        {
            reader.ReadOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motions[i].ReadExtended(reader);
                }
            });
        }

        foreach ((string? Name, int FamilyIdx, int CastIdx) in castInfo)
        {
            Families[FamilyIdx].Casts[CastIdx].Name = Name;
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Version);
        writer.WriteSingle(Priority);
        writer.WriteSingle(FrameRate);
        writer.WriteSingle(Motions.Count != 0 ? Motions.Min(x => x.Value.StartFrame) : 0);
        writer.WriteSingle(Motions.Count != 0 ? Motions.Max(x => x.Value.EndFrame) : 0);

        writer.WriteInt32(Textures.Count);
        writer.WriteOffset(() => writer.WriteVector2Array(Textures.ToArray()));

        writer.WriteInt32(Sprites.Count);
        writer.WriteObjectCollectionOffset(Sprites);

        writer.WriteInt32(Families.Count);
        writer.WriteOffset(() =>
        {
            foreach (Family family in Families)
            {
                writer.WriteObject(family);
            }
        });

        writer.WriteObject(BuildCastTable());
        writer.WriteObject(Motions);

        if (Version >= 1)
        {
            writer.WriteSingle(AspectRatio);
        }

        if (Version >= 2)
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    writer.WriteSingle(Motions[i].StartFrame);
                    writer.WriteSingle(Motions[i].EndFrame);
                }
            });
        }

        if (Version >= 3)
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motions[i].WriteExtended(writer);
                }
            });
        }
    }

    public CastInfoTable BuildCastTable()
    {
        CastInfoTable result = [];
        for (int i = 0; i < Families.Count; i++)
        {
            Family family = Families[i];
            for (int c = 0; c < family.Casts.Count; c++)
            {
                result.Add(new(family.Casts[c].Name, i, c));
            }
        }

        // Sort because game uses binary search to look for casts
        result.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        return result;
    }
}