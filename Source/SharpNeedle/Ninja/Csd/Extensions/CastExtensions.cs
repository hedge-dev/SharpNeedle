namespace SharpNeedle.Ninja.Csd.Extensions;

public static class CastExtensions
{
    public static SurfRide.Draw.CastNode ToSrdCastNode(this Cast cast, CastInfo parentCastInfo, Family.TreeDescriptorNode[] tree, SurfRide.Draw.Layer srdLayer, List<SurfRide.Draw.Font> srdFonts)
    {
        var node = tree[cast.Priority];
        var srdCast = new SurfRide.Draw.CastNode()
        {
            Name = cast.Name,
            ID = ++CsdProjectExtensions.GlobalID,
            Flags = cast.Field04 != 0 ? SurfRide.Draw.CastNode.CastNodeAttribute.ImageCast : SurfRide.Draw.CastNode.CastNodeAttribute.NullCast,
            Data = cast.Field04 != 0 ? ConvertImageData() : null,
            ChildIndex = (short)node.ChildIndex,
            SiblingIndex = (short)node.SiblingIndex,
            Layer = srdLayer,
            Priority = cast.Priority
        };

        srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.HasIlluminationColor;
        srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.BlendDisplayFlag;

        srdCast.Cell = ConvertTransformToCell();

        foreach (var child in cast)
            srdCast.Add(child.ToSrdCastNode(parentCastInfo, tree, srdLayer, srdFonts));

        return srdCast;

        SurfRide.Draw.ImageCastData ConvertImageData()
        {
             var data = new SurfRide.Draw.ImageCastData();

            if (cast.Flags.Test(0))
                data.Flags |= SurfRide.Draw.CastAttribute.BlendModeAdd;

            if (cast.Flags.Test(10))
                data.Flags |= SurfRide.Draw.CastAttribute.FlipHorizontal;

            if (cast.Flags.Test(11))
                data.Flags |= SurfRide.Draw.CastAttribute.FlipVertical;

            if (cast.Flags.Test(12))
                data.Flags |= SurfRide.Draw.CastAttribute.AntiAliasing;

            data.Size = (cast.BottomRight - cast.TopLeft) * srdLayer.Scene.Resolution;

            // Ninja Cell Sprite Draw Projects store color in a different order compared to SurfRide Draw Projects so it has to be flipped around
            data.VertexColorTopLeft = CsdProjectExtensions.Endianness == Endianness.Big ? cast.Info.GradientTopLeft : new(cast.Info.GradientTopLeft.A, cast.Info.GradientTopLeft.B, cast.Info.GradientTopLeft.G, cast.Info.GradientTopLeft.R);
            data.VertexColorTopRight = CsdProjectExtensions.Endianness == Endianness.Big ? cast.Info.GradientTopRight : new(cast.Info.GradientTopRight.A, cast.Info.GradientTopRight.B, cast.Info.GradientTopRight.G, cast.Info.GradientTopRight.R);
            data.VertexColorBottomLeft = CsdProjectExtensions.Endianness == Endianness.Big ? cast.Info.GradientBottomLeft : new(cast.Info.GradientBottomLeft.A, cast.Info.GradientBottomLeft.B, cast.Info.GradientBottomLeft.G, cast.Info.GradientBottomLeft.R);
            data.VertexColorBottomRight = CsdProjectExtensions.Endianness == Endianness.Big ? cast.Info.GradientBottomRight : new(cast.Info.GradientBottomRight.A, cast.Info.GradientBottomRight.B, cast.Info.GradientBottomRight.G, cast.Info.GradientBottomRight.R);
            
            data.Surface.CropIndex = (short)cast.Info.SpriteIndex;
            data.Surface1.CropIndex = -1;

            foreach (var spriteIndex in cast.SpriteIndices)
            {
                data.Surface.CropRefs.Add(new()
                {
                    TextureIndex = spriteIndex == -1 ? (short)-1 : (short)cast.Family.Scene.Sprites[spriteIndex].TextureIndex,
                    CropIndex = (short)spriteIndex
                });
            }

            if (cast.Field04 == 2)
                ConvertFontData(data);

            // The TopLeft/BottomRight values of Ninja Cell Sprite Draw Project Casts can be used to convert into the SurfRide Draw Project Pivot Position Attribute
            data.PivotPoint = -cast.TopLeft * srdLayer.Scene.Resolution;
            data.Flags |= SurfRide.Draw.CastAttribute.PivotPositionCustom;

            return data;
        }

        void ConvertFontData(SurfRide.Draw.ImageCastData imageData)
        {
            imageData.Flags |= SurfRide.Draw.CastAttribute.UseFont;

            imageData.TextData = new()
            {
                Field00 = 19,
                Scale = new Vector2(1.0f, 1.0f),
                Text = cast.Text,
                Field16 = -500,
                SpaceCorrection = (short)(cast.Field4C * srdLayer.Scene.Resolution.X),
                Field1E = 16
            };

            for (int i = 0; i < srdFonts.Count; i++)
            {
                if (srdFonts[i].Name != cast.FontName)
                    continue;

                imageData.TextData.FontIndex = i;
                imageData.TextData.Font = srdFonts[i];
            }
        }

        SurfRide.Draw.Cell ConvertTransformToCell()
        {
            var castPos = cast.Info.Translation + cast.Origin;
            var castRot = cast.Info.Rotation;
            var castScale = cast.Info.Scale;
            var castColor = cast.Info.Color;

            if (cast.InheritanceFlags.Test(3))
                srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.BlendMaterialColor;

            parentCastInfo.Scale *= castScale;

            if ((!cast.InheritanceFlags.Test(10) && cast.Parent != null && cast.Parent.InheritanceFlags.Test(10)) &&
                (!cast.InheritanceFlags.Test(11) && cast.Parent != null && cast.Parent.InheritanceFlags.Test(11)))
                srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;

            if (cast.Parent != null && !cast.InheritanceFlags.Test(10) && cast.InheritanceFlags.Test(11))
            {
                if (cast.Info.Scale.X != 1 && cast.Info.Scale.Y == 1 && parentCastInfo.Scale.Y == cast.Info.Scale.Y)
                {
                    srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;
                }
                else
                {
                    if (parentCastInfo.Scale.Y == cast.Info.Scale.Y)
                    {
                        srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;
                    }
                    else if (parentCastInfo.Scale.X != 0)
                    {
                        castScale.X /= parentCastInfo.Scale.X;
                        parentCastInfo.Scale.X /= parentCastInfo.Scale.X;
                    }
                    else if (parentCastInfo.Scale.Y != 0)
                    {
                        srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;
                        castScale.Y *= parentCastInfo.Scale.Y;
                        parentCastInfo.Scale = castScale;
                    }
                }
            }

            if (cast.Parent != null && cast.InheritanceFlags.Test(10) && !cast.InheritanceFlags.Test(11))
            {
                if (cast.Info.Scale.X == 1 && cast.Info.Scale.Y != 1 && parentCastInfo.Scale.X == cast.Info.Scale.X)
                {
                    srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;
                }
                else
                {
                    if (parentCastInfo.Scale.X == cast.Info.Scale.X)
                    {
                        srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;
                    }
                    else if (parentCastInfo.Scale.Y != 0)
                    {
                        castScale.Y /= parentCastInfo.Scale.Y;
                        parentCastInfo.Scale.Y /= parentCastInfo.Scale.Y;
                    }
                    else if (parentCastInfo.Scale.X != 0)
                    {
                        srdCast.Flags |= SurfRide.Draw.CastNode.CastNodeAttribute.LocalScale;
                        castScale.X *= parentCastInfo.Scale.X;
                        parentCastInfo.Scale = castScale;
                    }
                }
            }

            if (cast.Parent == null)
                castPos -= new Vector2(0.5f, 0.5f);

            return new SurfRide.Draw.Cell()
            {
                MaterialColor = CsdProjectExtensions.Endianness == Endianness.Big ? castColor : new(castColor.A, castColor.B, castColor.G, castColor.R),
                IlluminationColor = new(0, 0, 0, 255),
                IsVisible = cast.Info.HideFlag != 1 ? true : false,
                Translation = new(castPos.X * srdLayer.Scene.Resolution.X, -castPos.Y * srdLayer.Scene.Resolution.Y, 0.0f),
                RotationZ = (int)(castRot * 65535.0f / 360.0f),

                Scale = new(castScale, 1.0f),
            };
        }
    }
}