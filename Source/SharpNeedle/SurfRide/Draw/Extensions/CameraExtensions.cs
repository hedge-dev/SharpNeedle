namespace SharpNeedle.SurfRide.Draw.Extensions;

public static class CameraExtensions
{
    public static void AdjustCameraPosition(this Camera camera, Vector3 position)
    {
        camera.Position = position;
    }
}
