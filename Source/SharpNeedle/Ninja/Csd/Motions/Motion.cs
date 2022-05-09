namespace SharpNeedle.Ninja.Csd.Motions;

public class Motion : IBinarySerializable
{
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public List<LayerMotion> LayerMotions { get; set; }
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
        
        for (int i = 0; i < LayerMotions.Count; i++)
            scene.Layers[i].AttachMotion(LayerMotions[i]);

        // Attach to layers that we don't have
        for (int i = LayerMotions.Count; i < scene.Layers.Count; i++)
            LayerMotions.Add(new LayerMotion(scene.Layers[i]));
    }

    public void Read(BinaryObjectReader reader)
    {
        LayerMotions = reader.ReadObject<BinaryList<LayerMotion>>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        // Sanity checks
        for (int i = 0; i < LayerMotions.Count && i < Scene.Layers.Count; i++)
        {
            if (Scene.Layers[i] == LayerMotions[i].Layer)
                continue;

            // Re-arrange motions to fit the palette
            var idx = Scene.Layers.IndexOf(LayerMotions[i].Layer);
            var temp = LayerMotions[i];
            LayerMotions[idx] = LayerMotions[i];
            LayerMotions[i] = temp;
        }

        for (int i = LayerMotions.Count; i < Scene.Layers.Count; i++)
            LayerMotions.Add(new LayerMotion(Scene.Layers[i]));

        writer.WriteObject<BinaryList<LayerMotion>>(LayerMotions);
    }

    public void ReadExtended(BinaryObjectReader reader)
    {
        reader.ReadOffset(() =>
        {
            foreach (var motion in LayerMotions)
                motion.ReadExtended(reader);
        });
    }

    public void WriteExtended(BinaryObjectWriter writer)
    {
        writer.WriteOffset(() =>
        {
            foreach (var motion in LayerMotions)
                motion.WriteExtended(writer);
        });
    }
}