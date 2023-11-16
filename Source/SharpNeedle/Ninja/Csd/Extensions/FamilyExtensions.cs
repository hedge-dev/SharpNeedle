namespace SharpNeedle.Ninja.Csd.Extensions;

public static class FamilyExtensions
{
    public static SurfRide.Draw.Layer ToSrdLayer(this Family family, string familyName, SurfRide.Draw.Scene srdScene, List<SurfRide.Draw.Font> srdFonts)
    {
        var srdLayer = new SurfRide.Draw.Layer()
        {
            Name = familyName,
            ID = ++CsdProjectExtensions.GlobalID,
            Flags = (int)SurfRide.Draw.LayerAttribute.Is3D,
            Is3D = true,
            Scene = srdScene
        };

        var nodes = new Family.TreeDescriptorNode[family.Casts.Count];

        // Initialise items because, the runtime doesn't
        for (int i = 0; i < nodes.Length; i++)
            nodes[i] = new Family.TreeDescriptorNode();

        BuildTree(family);

        foreach (var cast in family)
            srdLayer.Add(cast.ToSrdCastNode(new() { Scale = new(1.0f, 1.0f), Color = new(255, 255, 255, 255) }, nodes, srdLayer, srdFonts));

        for (int i = 0; i < family.Casts.Count; i++)
            srdLayer.Casts[i].Parent = family.Casts[i].Parent != null ? srdLayer.Casts[family.Casts[i].Parent.Priority] : null;

        foreach (var motion in family.Scene.Motions)
            srdLayer.Animations.Add(motion.Value.ToSrdAnimation(motion.Key, family, srdLayer));

        return srdLayer;

        void BuildTree(IList<Cast> casts)
        {
            for (int i = 0; i < casts.Count; i++)
            {
                var cast = casts[i];
                var idx = family.Casts.IndexOf(cast);

                if (cast.Children.Count > 0)
                    nodes[idx].ChildIndex = family.Casts.IndexOf(cast.Children[0]);

                if (i + 1 < casts.Count)
                    nodes[idx].SiblingIndex = family.Casts.IndexOf(casts[i + 1]);

                BuildTree(cast);
            }
        }
    }
}