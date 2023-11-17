using SharpNeedle.Ninja.Csd;

namespace SharpNeedle.SurfRide.Draw.Extensions;

public static class LayerExtensions
{
    public static void SwapParentWithChild(this Layer lyr, CastNode parent, CastNode child)
    {
        var parentPrio = parent.Priority;
        parent.Priority = child.Priority;
        child.Priority = parentPrio;

        var parentVisibility = parent.Cell.IsVisible;
        parent.Cell.IsVisible = child.Cell.IsVisible;
        child.Cell.IsVisible = parentVisibility;

        // Remove the Children from the Parent cast to separate the hierarchy
        parent.Children.Remove(child);
        child.Parent = parent.Parent;

        // Add the Parent to the Children to reverse the hiearchy
        if (child.Children.Count != 0)
            child.Children[child.Children.Count - 1].SiblingIndex = (short)parent.Priority;

        parent.Parent.Children.Remove(parent);
        parent.Parent.Children.Add(child);

        child.Children.Add(parent);
        parent.Parent = child;

        short[] childIndicies = new short[] { child.ChildIndex, child.SiblingIndex };
        child.ChildIndex = parent.ChildIndex;
        child.SiblingIndex = parent.SiblingIndex;

        parent.SiblingIndex = childIndicies[0];
        parent.ChildIndex = childIndicies[1];

        // The color inheritence gets removed of the new child.
        parent.Flags &= ~CastNode.CastNodeAttribute.BlendMaterialColor;

        SwapAnimations();
        SwapPositions();
        SwapScale();

        void SwapPositions()
        {
            var childPos = child.Cell.Translation;
            child.Cell.Translation = parent.Cell.Translation + childPos / 2.0f;
            parent.Cell.Translation = -childPos;
        }

        void SwapScale()
        {
            var originalChildScale = child.Cell.Scale;
            child.Cell.Scale = parent.Cell.Scale;
            parent.Cell.Scale = originalChildScale;

            var childFlags = child.Flags;
            if ((parent.Flags & CastNode.CastNodeAttribute.LocalScale) == CastNode.CastNodeAttribute.LocalScale)
                child.Flags |= CastNode.CastNodeAttribute.LocalScale;
            else
                child.Flags &= ~CastNode.CastNodeAttribute.LocalScale;

            if ((childFlags & CastNode.CastNodeAttribute.LocalScale) == CastNode.CastNodeAttribute.LocalScale)
                parent.Flags |= CastNode.CastNodeAttribute.LocalScale;
            else
                parent.Flags &= ~CastNode.CastNodeAttribute.LocalScale;
        }

        void SwapAnimations()
        {
            foreach (var animation in lyr.Animations)
            {
                foreach (var motion in animation)
                {
                    if (motion.CastID == child.ID)
                    {
                        motion.CastID = (ushort)parent.ID;
                        motion.Cast = parent;
                    }
                    else if (motion.CastID == parent.ID)
                    {
                        motion.CastID = (ushort)child.ID;
                        motion.Cast = child;
                    }

                    /*foreach (var track in motion.Tracks)
                    {
                        foreach (var keyframe in track)
                        {
                            if (track.CurveType == FCurveType.Sx)
                                keyframe.Value *= child.Cell.Scale.X;
                        
                            if (track.CurveType == FCurveType.Sy)
                                keyframe.Value *= child.Cell.Scale.Y;
                        }
                    }*/
                }
            }
        }
    }

    public static void TransferChild(this Layer lyr, CastNode cast, CastNode target)
    {
        var parent = cast.Parent;

        cast.Parent = target;

        target.Children.Add(cast);
        if (target.Children.Count == 1)
            target.ChildIndex = (short)cast.Priority;
        if (target.Children.Count >= 2)
            target.Children[target.Children.IndexOf(cast) - 1].SiblingIndex = (short)cast.Priority;

        parent.Children.Remove(cast);
        if (parent.Children.Count == 0)
            parent.ChildIndex = -1;
        // TODO: change sibling index of the cast before to the next cast


        cast.Cell.Translation = GetAbsolutePosition(cast.Cell.Translation / 2.0f, parent);
        cast.Cell.Translation = GetRelativePosition(cast.Cell.Translation, target);

        cast.Cell.Scale = GetAbsoluteScale(cast.Cell.Scale, parent);
        cast.Cell.Scale = new(MathF.Round(cast.Cell.Scale.X, 5), MathF.Round(cast.Cell.Scale.Y, 5), MathF.Round(cast.Cell.Scale.Z, 5));

        cast.Cell.Scale = GetRelativeScale(cast.Cell.Scale, target);

        Vector3 GetAbsolutePosition(Vector3 relativePosition, CastNode parent)
        {
            relativePosition += parent.Cell.Translation;

            if (parent.Parent != null)
                relativePosition = GetAbsolutePosition(relativePosition, parent.Parent);

            return relativePosition;
        }

        Vector3 GetRelativePosition(Vector3 absolutePosition, CastNode parent)
        {
            absolutePosition -= parent.Cell.Translation;

            if (parent.Parent != null)
                absolutePosition = GetRelativePosition(absolutePosition, parent.Parent);

            return absolutePosition;
        }

        Vector3 GetAbsoluteScale(Vector3 relativeScale, CastNode parent)
        {
            relativeScale *= parent.Cell.Scale;

            if (parent.Parent != null)
                relativeScale = GetAbsoluteScale(relativeScale, parent.Parent);

            return relativeScale;
        }

        Vector3 GetRelativeScale(Vector3 absoluteScale, CastNode parent)
        {
            absoluteScale /= parent.Cell.Scale;

            if (parent.Parent != null)
                absoluteScale = GetRelativeScale(absoluteScale, parent.Parent);

            return absoluteScale;
        }
    }
}
