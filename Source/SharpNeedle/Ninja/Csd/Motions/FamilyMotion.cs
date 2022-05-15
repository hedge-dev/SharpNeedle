namespace SharpNeedle.Ninja.Csd.Motions;

public class FamilyMotion : IBinarySerializable
{
    public List<CastMotion> CastMotions { get; set; } = new();
    public Family Family { get; internal set; }

    public FamilyMotion()
    {

    }

    public FamilyMotion(Family family)
    {
        family.AttachMotion(this);
    }

    public void OnAttach(Family family)
    {
        for (int i = CastMotions.Count; i < family.Casts.Count; i++)
            CastMotions.Add(new CastMotion(family.Casts[i]));
        
        for (int i = 0; i < family.Casts.Count; i++)
            family.Casts[i].AttachMotion(CastMotions[i]);
    }

    public void Read(BinaryObjectReader reader)
    {
        CastMotions = reader.ReadObject<BinaryList<CastMotion>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        // Remove casts we don't have
        CastMotions.RemoveAll(x => !Family.Casts.Contains(x.Cast));
        
        // Sanity checks
        for (int i = CastMotions.Count; i < Family.Casts.Count; i++)
            CastMotions.Add(new CastMotion(Family.Casts[i]));
        
        for (int i = 0; i < CastMotions.Count; i++)
        {
            if (Family.Casts[i] == CastMotions[i].Cast)
                continue;

            // Re-arrange motions
            var idx = Family.Casts.IndexOf(CastMotions[i].Cast);
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