namespace SharpNeedle.Ninja.Csd;
using Animation;
using FrameInfo = ValueTuple<float, float>;

public class Scene : IBinarySerializable
{
    public int Version { get; set; } = 3;
    public float ZIndex { get; set; }
    public float AnimationFrameRate { get; set; }
    public float AspectRatio { get; set; } = 1.333333F; // Default Aspect Ratio is 4:3
    public List<Vector2> Textures { get; set; } // Size of textures used in the scene
    public List<Sprite> Sprites { get; set; }
    public List<Layer> Layers { get; set; }
    public CsdDictionary<MotionPalette> Motions { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Version = reader.Read<int>();
        ZIndex = reader.Read<float>();
        AnimationFrameRate = reader.Read<float>();
        var motionStart = reader.Read<float>();
        var motionLength = reader.Read<float>();
        Textures = new List<Vector2>(reader.ReadArrayOffset<Vector2>(reader.Read<int>()));
        Sprites = new List<Sprite>(reader.ReadArrayOffset<Sprite>(reader.Read<int>()));
        Layers = reader.ReadObject<BinaryList<Layer>>();
        var castInfo = reader.ReadObject<CastDictionary>();
        Motions = reader.ReadObject<CsdDictionary<MotionPalette>>();

        foreach (var motion in Motions)
            motion.Value.Attach(this);

        for (int i = 0; i < Motions.Count; i++)
        {
            Motions[i].Start = motionStart;
            Motions[i].Length = motionLength;
        }

        if (Version >= 1)
            AspectRatio = reader.Read<float>();

        if (Version >= 2)
        {
            var frameInfo = reader.ReadArrayOffset<FrameInfo>(Motions.Count);
            for (int i = 0; i < Motions.Count; i++)
            {
                Motions[i].Start = frameInfo[i].Item1;
                Motions[i].Length = frameInfo[i].Item2;
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
            Layers[item.LayerIdx].Casts[item.CastIdx].Name = item.Name;
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Version);
        writer.Write(ZIndex);
        writer.Write(AnimationFrameRate);
        writer.Write(Motions.Min(x => x.Value.Start));
        writer.Write(Motions.Max(x => x.Value.Length));

        writer.Write(Textures.Count);
        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Textures).AsMemory());

        writer.Write(Sprites.Count);
        writer.WriteArrayOffset(CollectionsMarshal.AsSpan(Sprites).AsMemory());

        writer.WriteObject<BinaryList<Layer>>(Layers);
        writer.WriteObject(BuildCastDictionary());
        writer.WriteObject(Motions);

        if (Version >= 1)
            writer.Write(AspectRatio);

        if (Version >= 2)
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Motions.Count; i++)
                    writer.Write(new FrameInfo(Motions[i].Start, Motions[i].Length));
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

    public CastDictionary BuildCastDictionary()
    {
        var result = new CastDictionary();
        for (int i = 0; i < Layers.Count; i++)
        {
            var layer = Layers[i];
            for (int c = 0; c < layer.Casts.Count; c++)
            {
                result.Add(new (layer.Casts[c].Name, i, c));
            }
        }

        // Sort because game uses binary search to look for casts
        result.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        return result;
    }
}