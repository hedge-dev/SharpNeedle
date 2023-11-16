using SharpNeedle.Ninja.Csd.Motions;

namespace SharpNeedle.Ninja.Csd.Extensions;

public static class MotionExtensions
{
    public static SurfRide.Draw.Animation ToSrdAnimation(this Motion motion, string motionName, Family family, SurfRide.Draw.Layer srdLayer)
    {
        var srdAnimation = new SurfRide.Draw.Animation()
        {
            Name = motionName,
            ID = ++CsdProjectExtensions.GlobalID,
            EndFrame = Convert.ToUInt32(motion.EndFrame),
            Layer = srdLayer
        };

        foreach (var familyMotion in motion.FamilyMotions)
        {
            if (familyMotion.Family != family)
                continue;

            for (int i = 0; i < familyMotion.CastMotions.Count; i++)
            {
                var cast = srdLayer.Casts[familyMotion.CastMotions[i].Cast.Priority];
                if (cast == null)
                    continue;

                srdAnimation.Add(new() { Animation = srdAnimation, Cast = cast, CastID = (ushort)cast.ID });

                foreach (var keyframeList in familyMotion.CastMotions[i])
                {
                    var curveType = new SurfRide.Draw.FCurveType();
                    switch (keyframeList.Property)
                    {
                        case KeyProperty.HideFlag:
                            curveType = SurfRide.Draw.FCurveType.Display;
                            break;
                        case KeyProperty.PositionX:
                            curveType = SurfRide.Draw.FCurveType.Tx;
                            break;
                        case KeyProperty.PositionY:
                            curveType = SurfRide.Draw.FCurveType.Ty;
                            break;
                        case KeyProperty.Rotation:
                            curveType = SurfRide.Draw.FCurveType.Rz;
                            break;
                        case KeyProperty.ScaleX:
                            curveType = SurfRide.Draw.FCurveType.Sx;
                            break;
                        case KeyProperty.ScaleY:
                            curveType = SurfRide.Draw.FCurveType.Sy;
                            break;
                        case KeyProperty.SpriteIndex:
                            curveType = SurfRide.Draw.FCurveType.CropIndex0;
                            break;
                        case KeyProperty.Color:
                            curveType = SurfRide.Draw.FCurveType.MaterialColor;
                            break;
                        case KeyProperty.GradientTopLeft:
                            curveType = SurfRide.Draw.FCurveType.VertexColorTopLeft;
                            break;
                        case KeyProperty.GradientBottomLeft:
                            curveType = SurfRide.Draw.FCurveType.VertexColorBottomLeft;
                            break;
                        case KeyProperty.GradientTopRight:
                            curveType = SurfRide.Draw.FCurveType.VertexColorTopRight;
                            break;
                        case KeyProperty.GradientBottomRight:
                            curveType = SurfRide.Draw.FCurveType.VertexColorBottomRight;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    var srdTrack = new SurfRide.Draw.Track()
                    {
                        CurveType = curveType,
                        StartFrame = Convert.ToUInt32(motion.StartFrame),
                        EndFrame = Convert.ToUInt32(motion.EndFrame)
                    };

                    switch (srdTrack.CurveType)
                    {
                        case SurfRide.Draw.FCurveType.CropIndex0:
                        case SurfRide.Draw.FCurveType.CropIndex1:
                            srdTrack.Flags |= 0x20;
                            break;
                        case SurfRide.Draw.FCurveType.Display:
                            srdTrack.Flags |= 0x30;
                            break;
                        case SurfRide.Draw.FCurveType.MaterialColor:
                        case SurfRide.Draw.FCurveType.VertexColorTopLeft:
                        case SurfRide.Draw.FCurveType.VertexColorTopRight:
                        case SurfRide.Draw.FCurveType.VertexColorBottomLeft:
                        case SurfRide.Draw.FCurveType.VertexColorBottomRight:
                        case SurfRide.Draw.FCurveType.IlluminationColor:
                            srdTrack.Flags |= 0x50;
                            break;
                        default:
                            srdTrack.Flags |= 0x10;
                            break;
                    }

                    foreach (var keyframe in keyframeList)
                    {
                        SurfRide.Draw.InterpolationType interpolationType = new();

                        switch (keyframe.Interpolation)
                        {
                            case InterpolationType.Const:
                                interpolationType = SurfRide.Draw.InterpolationType.Constant;
                                break;
                            case InterpolationType.Linear:
                                interpolationType = SurfRide.Draw.InterpolationType.Linear;
                                break;
                            case InterpolationType.Hermite:
                                interpolationType = SurfRide.Draw.InterpolationType.Hermite;
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        srdTrack.Flags |= (int)interpolationType;

                        var srdKeyframe = new SurfRide.Draw.KeyFrame()
                        {
                            Frame = (int)keyframe.Frame,
                            InParam = keyframe.InTangent,
                            OutParam = keyframe.OutTangent,
                            Field10 = (int)keyframe.Field14
                        };

                        switch ((srdTrack.Flags & 0xF0))
                        {
                            case 0x10:
                                srdKeyframe.Value = keyframe.Value.Float;
                                break;
                            case 0x20:
                                srdKeyframe.Value = Convert.ToInt32(keyframe.Value.Float);
                                break;
                            case 0x30:
                                srdKeyframe.Value = Convert.ToBoolean(keyframe.Value.Float);
                                break;
                            case 0x50:
                                srdKeyframe.Value = new Color<byte>(keyframe.Value.Color.A, keyframe.Value.Color.B, keyframe.Value.Color.G, keyframe.Value.Color.R);
                                break;
                        }

                        if (srdTrack.CurveType == SurfRide.Draw.FCurveType.Tx)
                        {
                            if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin.X == 0.0f)
                                srdKeyframe.Value -= 0.5f;
                            else if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin.X == 1.0f)
                                srdKeyframe.Value += 0.5f;

                            srdKeyframe.Value *= srdLayer.Scene.Resolution.X;
                        }
                        else if (srdTrack.CurveType == SurfRide.Draw.FCurveType.Ty)
                        {
                            if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin == new Vector2(0.0f, 0.0f))
                                srdKeyframe.Value -= 0.5f;
                            else if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin.X == 1.0f)
                                srdKeyframe.Value += 0.5f;

                            srdKeyframe.Value *= -srdLayer.Scene.Resolution.Y;
                        }
                        else if (srdTrack.CurveType == SurfRide.Draw.FCurveType.Display)
                        {
                            srdKeyframe.Value = !srdKeyframe.Value.Boolean;
                        }
                        else if (srdTrack.CurveType == SurfRide.Draw.FCurveType.Rz)
                        {
                            srdKeyframe.Value = Convert.ToInt32(srdKeyframe.Value.Float * 65535.0f / 360.0f);
                        }

                        srdTrack.Add(srdKeyframe);
                    }

                    if (srdTrack.CurveType == SurfRide.Draw.FCurveType.Rz)
                    {
                        srdTrack.Flags &= ~0xF0;
                        srdTrack.Flags |= 0x40;
                    }

                    srdAnimation[i].Tracks.Add(srdTrack);
                }
            }
        }

        for (int i = srdAnimation.Count - 1; i >= 0; i--)
            if (srdAnimation[i].Tracks.Count == 0)
                srdAnimation.RemoveAt(i);

        return srdAnimation;
    }
}