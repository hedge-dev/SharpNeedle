namespace SharpNeedle.Ninja.Csd.Motions;

public class LayerMotion : IBinarySerializable
{
    public List<CastMotion> CastMotions { get; set; } = new();
    public Layer Layer { get; internal set; }

    public LayerMotion()
    {

    }

    public LayerMotion(Layer layer)
    {
        layer.AttachMotion(this);
    }

    public void OnAttach(Layer layer)
    {
        for (int i = CastMotions.Count; i < layer.Casts.Count; i++)
            CastMotions.Add(new CastMotion(layer.Casts[i]));
        
        for (int i = 0; i < layer.Casts.Count; i++)
            layer.Casts[i].AttachMotion(CastMotions[i]);
    }

    public void Read(BinaryObjectReader reader)
    {
        CastMotions = reader.ReadObject<BinaryList<CastMotion>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        // Sanity checks
        for (int i = CastMotions.Count; i < Layer.Casts.Count; i++)
            CastMotions.Add(new CastMotion(Layer.Casts[i]));
        
        for (int i = 0; i < CastMotions.Count; i++)
        {
            if (Layer.Casts[i] == CastMotions[i].Cast)
                continue;

            // Re-arrange motions
            var idx = Layer.Casts.IndexOf(CastMotions[i].Cast);
            var temp = CastMotions[i];
            CastMotions[idx] = CastMotions[i];
            CastMotions[i] = temp;
        }

        writer.WriteObject<BinaryList<CastMotion>>(CastMotions);
    }

    public void ReadExtended(BinaryObjectReader reader)
    {
        var unused = reader.Read<int>();
        reader.ReadOffset(() =>
        {
            reader.ReadOffset(() =>
            {
                reader.ReadOffset(() =>
                {
                    foreach (var motion in CastMotions)
                        motion.ReadExtended(reader);
                });
            });
        });
    }

    public void WriteExtended(BinaryObjectWriter writer)
    {
        writer.Write<int>(0);
        writer.WriteOffset(() =>
        {
            writer.WriteOffset(() =>
            {
                writer.WriteOffset(() =>
                {
                    foreach (var motion in CastMotions)
                        motion.WriteExtended(writer);
                });
            });
        });
    }
}