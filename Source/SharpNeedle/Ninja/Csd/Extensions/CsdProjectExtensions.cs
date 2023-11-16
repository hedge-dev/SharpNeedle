using System.IO;

namespace SharpNeedle.Ninja.Csd.Extensions;

public static class CsdProjectExtensions
{
    public static int GlobalID = 0;
    public static Endianness Endianness = Endianness.Little;

    public static SurfRide.Draw.SrdProject ToSrdProject(this CsdProject csd)
    {
        Endianness = csd.Endianness;
        var srd = new SurfRide.Draw.SrdProject()
        {
            TextureLists = new(),
            Endianness = Endianness
        };

        foreach (var scene in csd.Project.Root.Scenes)
            srd.TextureLists.Add(csd.ToSrdTextureList(scene.Key, scene.Value));

        foreach (var childNode in csd.Project.Root.Children)
            foreach (var childScene in childNode.Value.Scenes)
                srd.TextureLists.Add(csd.ToSrdTextureList(childScene.Key, childScene.Value));
        
        // Add separate texture lists for Fonts so it's not pulling from a random scene's texture list in SurfRide Draw Project
        if (csd.Project.Fonts != null && csd.Project.Fonts.Count != 0)
        {
            for (int i = 0; i < csd.Project.Fonts.Count; i++)
            {
                // The very first scene in the final is passed down here because we only take texture dimensions and texture names out of it.
                var baseScene = csd.Project.Root.Children.Count != 0 ? csd.Project.Root.Children[0].Scenes[0] : csd.Project.Root.Scenes[0];
                srd.TextureLists.Add(csd.ToSrdTextureList($"Font_{i:D3}", baseScene));
            }
        }

        srd.Project = csd.Project.ToSrdProjectChunk(srd.TextureLists);

        return srd;
    }

    public static SurfRide.Draw.TextureList ToSrdTextureList(this CsdProject csd, string sceneName, Scene scene)
    {
        var resolution = scene.AspectRatio == 1.33333337f ? new Vector2(640.0f, 480.0f) : new Vector2(1280.0f, 720.0f);
        var texNames = GetTextureNamesWithoutExtensions();

        var srdTexList = new SurfRide.Draw.TextureList() { Name = sceneName };

        for (int i = 0; i < texNames.Count; i++)
        {
            srdTexList.Add(new()
            {
                Name = texNames[i],
                TextureFileName = texNames[i],
                ID = ++GlobalID,
                Width = (ushort)(scene.Textures[i].X * resolution.X),
                Height = (ushort)(scene.Textures[i].Y * resolution.Y),
                Flags = 0x110
            });

            CreateCrops(srdTexList, i);
        }

        return srdTexList;

        List<string> GetTextureNamesWithoutExtensions()
        {
            var names = new List<string>();

            if (csd.Textures == null || csd.Textures.Count == 0)
            {
                // If the texture format doesn't store the texture names in their respective Ninja Cell Sprite Draw Project files, names are generated based on the filename
                int texCount = scene.Sprites.Max(s => s.TextureIndex) + 1;
                for (int i = 0; i < texCount; i++)
                    names.Add($"{Path.ChangeExtension(csd.BaseFile.Name, null)}-{(i + 1):D3}");
            }
            else
            {
                // Ninja Cell Sprite Draw Projects store the texture name with the extension thus we strip the extension from the name before adding to SurfRide Draw Projects
                foreach (var tex in csd.Textures)
                    names.Add(Path.ChangeExtension(tex.Name, null));
            }

            return names;
        }

        void CreateCrops(SurfRide.Draw.TextureList texList, int index)
        {
            foreach (var sprite in scene.Sprites)
            {
                if (sprite.TextureIndex != index)
                    continue;

                texList.GetTexture(texNames[index]).Add(new()
                {
                    Left = sprite.TopLeft.X,
                    Top = sprite.TopLeft.Y,
                    Right = sprite.BottomRight.X,
                    Bottom = sprite.BottomRight.Y,
                });
            }
        }
    }
}