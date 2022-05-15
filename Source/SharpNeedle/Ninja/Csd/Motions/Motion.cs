namespace SharpNeedle.Ninja.Csd.Motions;

public class Motion : IBinarySerializable
{
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public List<FamilyMotion> FamilyMotions { get; set; }
    public Scene Scene { get; internal set; }

    public Motion()
    {

    }

    public Motion(Scene scene)
    {
        Attach(scene);
    }

    public void Attach(Scene scene)
    {
        Scene = scene;

        // Attach to families that we don't have
        for (int i = FamilyMotions.Count; i < scene.Families.Count; i++)
            FamilyMotions.Add(new FamilyMotion(scene.Families[i]));

        for (int i = 0; i < FamilyMotions.Count; i++)
            scene.Families[i].AttachMotion(FamilyMotions[i]);
    }

    public void Read(BinaryObjectReader reader)
    {
        FamilyMotions = reader.ReadObject<BinaryList<FamilyMotion>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        // Remove families we don't have
        FamilyMotions.RemoveAll(x => !Scene.Families.Contains(x.Family));
 
        // Sanity checks
        for (int i = FamilyMotions.Count; i < Scene.Families.Count; i++)
            FamilyMotions.Add(new FamilyMotion(Scene.Families[i]));
        
        for (int i = 0; i < FamilyMotions.Count; i++)
        {
            if (Scene.Families[i] == FamilyMotions[i].Family)
                continue;

            // Re-arrange motions to fit the palette
            var idx = Scene.Families.IndexOf(FamilyMotions[i].Family);
            var temp = FamilyMotions[i];
            FamilyMotions[idx] = FamilyMotions[i];
            FamilyMotions[i] = temp;
        }

        writer.WriteObject<BinaryList<FamilyMotion>>(FamilyMotions);
    }

    public void ReadExtended(BinaryObjectReader reader)
    {
        reader.ReadOffset(() =>
        {
            foreach (var motion in FamilyMotions)
                motion.ReadExtended(reader);
        });
    }

    public void WriteExtended(BinaryObjectWriter writer)
    {
        writer.WriteOffset(() =>
        {
            foreach (var motion in FamilyMotions)
                motion.WriteExtended(writer);
        });
    }
}