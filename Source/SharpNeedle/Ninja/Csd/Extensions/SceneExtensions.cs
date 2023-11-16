namespace SharpNeedle.Ninja.Csd.Extensions;

public static class SceneExtensions
{
    public static SurfRide.Draw.Scene ToSrdScene(this Scene scene, string sceneName, List<SurfRide.Draw.Font> srdFonts)
    {
        var resolution = scene.AspectRatio == 1.33333337f ? new Vector2(640.0f, 480.0f) : new Vector2(1280.0f, 720.0f);
        
        var srdScene = new SurfRide.Draw.Scene()
        {
            Name = sceneName,
            ID = ++CsdProjectExtensions.GlobalID,
            Resolution = resolution,
            BackgroundColor = new(68, 68, 204, 255),
            Cameras = new()
            {
                new()
                {
                    Name = "default",
                    ID = ++CsdProjectExtensions.GlobalID,
                    FieldOfView = 0x2000,
                    Position = resolution.Equals(new(640.0f, 480.0f)) ? new(0.0f, 0.0f, 1000.0f) : new(0.0f, 0.0f, 869.12f),
                    NearPlane = 10.0f,
                    FarPlane = 100000.0f
                }
            }
        };

        for (int i = 0; i < scene.Families.Count; i++)
            srdScene.Layers.Add(scene.Families[i].ToSrdLayer($"Layer_{(i + 1):D3}", srdScene, srdFonts));

        return srdScene;
    }
}