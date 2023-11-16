namespace SharpNeedle.SurfRide.Draw.Extensions;

public static class ProjectExtensions
{
    public static void ChangeAspectRatio(this SrdProject srd, float aspectRatio, float tolerance = 0.0001f)
    {
        foreach (var scene in srd.Project.Scenes)
        {
            foreach (var layer in scene.Layers)
            {
                foreach (var animation in layer.Animations)
                    animation.ChangeAspectRatio(aspectRatio, tolerance);

                foreach (var cast in layer)
                    cast.ChangeAspectRatio(aspectRatio, tolerance);
            }
        }
    }

    public static void AdjustCameraPosition(this SrdProject srd, Vector3 position)
    {
        srd.Project.Camera.AdjustCameraPosition(position);

        foreach (var scene in srd.Project.Scenes)
            foreach (var camera in scene.Cameras)
                camera.AdjustCameraPosition(position);
    }
}
