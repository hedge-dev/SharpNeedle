namespace SharpNeedle.SurfRide.Draw.Extensions;

public static class CastNodeExtensions
{
    public static void ChangeAspectRatio(this CastNode cast, float aspectRatio, float tolerance = 0.0001f)
    {
        var scene = cast.Layer.Scene;
        var cell = cast.Cell;

        if (cast.Parent != null && (cast.Parent.Cell.Translation.X > tolerance || cast.Parent.Cell.Translation.X < -tolerance))
            return;

        if (cell.Translation.X > tolerance)
            cell.Translation += new Vector3(scene.GetRatioOffset(aspectRatio), 0.0f, 0.0f);
        else if (cell.Translation.X < -tolerance)
            cell.Translation -= new Vector3(scene.GetRatioOffset(aspectRatio), 0.0f, 0.0f);

        foreach (var child in cast)
            ChangeAspectRatio(child, aspectRatio, tolerance);
    }
}
