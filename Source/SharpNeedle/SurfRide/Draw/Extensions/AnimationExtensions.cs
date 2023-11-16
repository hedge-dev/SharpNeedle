using SharpNeedle.SurfRide.Draw.Animations;

namespace SharpNeedle.SurfRide.Draw.Extensions;

public static class AnimationExtensions
{
    public static void ChangeAspectRatio(this Animation animation, float aspectRatio, float tolerance = 0.0001f)
    {
        foreach (var motion in animation)
        {
            foreach (var track in motion.Tracks)
            {
                var scene = animation.Layer.Scene;
                var cast = motion.Cast;
                var cell = cast.Cell;

                if (track.CurveType != FCurveType.Tx)
                    continue;

                if (cast.Parent != null && (cast.Parent.Cell.Translation.X < tolerance || cast.Parent.Cell.Translation.X > -tolerance))
                    continue;

                foreach (var keyframe in track)
                {
                    if (cell.Translation.X > tolerance)
                        keyframe.Value += scene.GetRatioOffset(aspectRatio);
                    else if (cell.Translation.X < -tolerance)
                        keyframe.Value -= scene.GetRatioOffset(aspectRatio);
                }
            }
        }
    }
}