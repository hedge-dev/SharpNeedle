using SharpNeedle.SurfRide.Draw;

namespace SharpNeedle.SurfRide.Draw.Extensions;

public static class SceneExtensions
{
    public static float GetRatioOffset(this Scene scene, float aspectRatio)
    {
        if (aspectRatio >= 0.0f)
            return (aspectRatio * scene.Resolution.Y - scene.Resolution.X) / 2.0f;
        else
            return (aspectRatio * scene.Resolution.Y + scene.Resolution.X) / 2.0f;
    }
}
