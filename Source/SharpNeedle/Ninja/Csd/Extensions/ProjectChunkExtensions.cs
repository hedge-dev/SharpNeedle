namespace SharpNeedle.Ninja.Csd.Extensions;

public static class ProjectChunkExtensions
{
    public static SurfRide.Draw.ProjectChunk ToSrdProjectChunk(this ProjectChunk project, SurfRide.Draw.TextureListChunk texLists)
    {
        // The very first scene is used here to calculate the resolution for the camera's position.
        // This assumes that every single scene in the file shares the same aspect ratio
        var baseScene = project.Root.Children.Count != 0 ? project.Root.Children[0].Scenes[0] : project.Root.Scenes[0];
        var resolution = baseScene.AspectRatio == 1.33333337f ? new Vector2(640.0f, 480.0f) : new Vector2(1280.0f, 720.0f);

        var srdProject = new SurfRide.Draw.ProjectChunk()
        {
            Name = project.Name,
            FrameRate = baseScene.FrameRate,
            TextureLists = texLists,
            Camera = new()
            {
                Name = "default",
                ID = ++CsdProjectExtensions.GlobalID,
                FieldOfView = 0x2000,
                Position = resolution.Equals(new(640.0f, 480.0f)) ? new(0.0f, 0.0f, 1000.0f) : new(0.0f, 0.0f, 869.12f),
                NearPlane = 10.0f,
                FarPlane = 100000.0f
            }
        };

        int fontTexListIndex = project.Root.Scenes.Count;
        foreach (var childNode in project.Root.Children)
            fontTexListIndex += childNode.Value.Scenes.Count;

        if (project.Fonts != null)
        {
            foreach (var font in project.Fonts)
            {
                srdProject.Fonts.Add(font.Value.ToSrdFont(font.Key, baseScene, fontTexListIndex));
                fontTexListIndex++;
            }
        }

        foreach (var scene in project.Root.Scenes)
            srdProject.Scenes.Add(scene.Value.ToSrdScene(scene.Key, srdProject.Fonts));

        foreach (var childNode in project.Root.Children)
            foreach (var childScene in childNode.Value.Scenes)
                srdProject.Scenes.Add(childScene.Value.ToSrdScene(childScene.Key, srdProject.Fonts));

        AdjustCropTextureListIndices();

        return srdProject;

        void AdjustCropTextureListIndices()
        {
            for (int i = 0; i < srdProject.Scenes.Count; i++)
            {
                foreach (var layer in srdProject.Scenes[i].Layers)
                {
                    foreach (var cast in layer.Casts)
                    {
                        if (cast.Data == null)
                            continue;

                        for (int j = 0; j < ((SurfRide.Draw.ImageCastData)cast.Data).Surface.CropRefs.Count; j++)
                        {
                            int spriteCount = 0;
                            var cropRef = ((SurfRide.Draw.ImageCastData)cast.Data).Surface.CropRefs[j];

                            // Fix the Texture List Index of CropRefs as they are just saved as 0 on conversion
                            cropRef.TextureListIndex = (short)i;

                            // Fix crop indicies going out of range due to sprites being a single array in Ninja Cell Sprite Draw Projects
                            for (int k = 0; k < cropRef.TextureIndex; k++)
                                spriteCount += srdProject.TextureLists[i][k].Count;
                            cropRef.CropIndex -= (short)spriteCount;

                            ((SurfRide.Draw.ImageCastData)cast.Data).Surface.CropRefs[j] = cropRef;
                        }
                    }
                }
            }
        }
    }
}