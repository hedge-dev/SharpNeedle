/*using System.IO;
using SharpNeedle.Ninja.Csd;
using SharpNeedle.SurfRide.Draw;

namespace SharpNeedle.Utilities;

public static class CsdExtensions
{
    private static int _globalID = 0;
    private static int _index = 0;
    private static Vector2 _resolution = Vector2.Zero;
    private static Endianness _endianness = Endianness.Little;

    public static SrdProject ToSrdProject(this CsdProject csd)
    {
        // The code assumes that all scenes operate under the same aspect ratio thus we only take the first scene's aspect ratio into account.
        var baseScene = csd.Project.Root.Children.Count != 0 ? csd.Project.Root.Children[0].Scenes[0] : csd.Project.Root.Scenes[0];

        _resolution = ToResolution(baseScene.AspectRatio);
        _endianness = csd.Endianness;

        SrdProject srd = new()
        {
            TextureLists = csd.ToSrdTextureList(),
            Project = csd.Project.ToSrdProjectChunk()
        };
        srd.Project.TextureLists = srd.TextureLists;

        for (int i = 0; i < srd.Project.Scenes.Count; i++)
        {
            foreach (var layer in srd.Project.Scenes[i].Layers)
            {
                foreach (var node in layer.Casts)
                {
                    if (node.Data == null)
                        continue;

                    for (int j = 0; j < ((ImageCastData)node.Data).Surface0.CropRefs.Count; j++)
                    {
                        var crop = ((ImageCastData)node.Data).Surface0.CropRefs[j];
                        crop.TextureListIndex = (short)i;

                        int spriteCount = 0;
                        for (int k = 0; k < crop.TextureIndex; k++)
                            spriteCount += srd.Project.TextureLists[i][k].Count;
                        crop.CropIndex -= (short)spriteCount;

                        ((ImageCastData)node.Data).Surface0.CropRefs[j] = crop;

                    }
                }
            }
        }

        return srd;
    }

    public static TextureListChunk ToSrdTextureList(this CsdProject csd)
    {
        var baseScene = csd.Project.Root.Children.Count != 0 ? csd.Project.Root.Children[0].Scenes[0] : csd.Project.Root.Scenes[0];
        var texNames = GetTextureNamesWithoutExtension();

        TextureListChunk result = new() { };
        AddTextureListsRecursively(csd.Project.Root);

        // Add a separate texture list for Fonts so it's not pulling from a random scene's texture list in SurfRide Draw Project
        if (csd.Project.Fonts.Count == 0)
            return result;
        
        result.Add(new() { Name = "Font_001" });
        switch (csd.Project.TextureFormat)
        {
            case TextureFormat.Unknown:
            case TextureFormat.RenderWare:
                int texCount = 0;
                foreach (var sprite in baseScene.Sprites)
                    if (texCount < sprite.TextureIndex)
                        texCount = sprite.TextureIndex;

                AddTextureList(baseScene, "Font_001", texCount + 1);
                break;
            case TextureFormat.Mirage:
            case TextureFormat.NextNinja:
                AddTextureList(baseScene, "Font_001", csd.Textures.Count);
                break;
        }

        return result;

        void AddTextureList(Ninja.Csd.Scene scene, string sceneName, int texCount)
        {
            for (int i = 0; i < texCount; i++)
            {
                result.GetTextureList(sceneName).Add(new()
                {
                    Name = texNames[i],
                    ID = ++_globalID,
                    Width = (ushort)(scene.Textures[i].X * _resolution.X),
                    Height = (ushort)(scene.Textures[i].Y * _resolution.Y),
                    // TODO - Flags
                });

                CreateCropRectangles(scene.Sprites, result.GetTextureList(sceneName), i);
            }

            void CreateCropRectangles(List<Sprite> sprites, TextureList texlist, int index)
            {
                foreach (var sprite in sprites)
                {
                    if (sprite.TextureIndex != index)
                        continue;

                    texlist.GetTexture(texNames[index]).Add(new()
                    {
                        Left = sprite.TopLeft.X,
                        Top = sprite.TopLeft.Y,
                        Right = sprite.BottomRight.X,
                        Bottom = sprite.BottomRight.Y,
                    });
                }
            }
        }

        void AddTextureListsRecursively(SceneNode sceneNode)
        {
            foreach (var scene in sceneNode.Scenes)
            {
                result.Add(new() { Name = scene.Key });

                switch (csd.Project.TextureFormat)
                {
                    case TextureFormat.Mirage:
                    case TextureFormat.NextNinja:
                        AddTextureList(scene.Value, scene.Key, csd.Textures.Count);
                        break;
                    case TextureFormat.Unknown:
                    case TextureFormat.RenderWare:
                        int texCount = 0;
                        foreach (var sprite in baseScene.Sprites)
                            if (texCount < sprite.TextureIndex)
                                texCount = sprite.TextureIndex;

                        AddTextureList(scene.Value, scene.Key, texCount + 1);
                        break;
                }
            }

            foreach (var child in sceneNode.Children)
                AddTextureListsRecursively(child.Value);
        }

        List<string> GetTextureNamesWithoutExtension()
        {
            var names = new List<string>();

            switch (csd.Project.TextureFormat)
            {
                case TextureFormat.Unknown:
                case TextureFormat.RenderWare:
                    int texCount = 0;
                    foreach (var sprite in baseScene.Sprites)
                        if (texCount < sprite.TextureIndex)
                            texCount = sprite.TextureIndex;

                    for (int i = 0; i < texCount + 1; i++)
                        names.Add(Path.ChangeExtension(csd.BaseFile.Name, null) + "-" + i);
                    break;
                case TextureFormat.Mirage:
                case TextureFormat.NextNinja:
                    // Ninja Cell Sprite Draw Projects store the texture name with the extension thus we strip the extension from the name before adding to SurfRide Draw Projects
                    foreach (var tex in csd.Textures)
                        names.Add(Path.ChangeExtension(tex.Name, null));

                    break;
            }

            return names;
        }
    }

    public static SurfRide.Draw.ProjectChunk ToSrdProjectChunk(this Ninja.Csd.ProjectChunk csdProject)
    {
        var baseScene = csdProject.Root.Children.Count != 0 ? csdProject.Root.Children[0].Scenes[0] : csdProject.Root.Scenes[0];
        
        SurfRide.Draw.ProjectChunk result = new()
        {
            Name = csdProject.Name,
            FrameRate = baseScene.FrameRate,
            Camera = new()
            {
                Name = "default",
                ID = ++_globalID,
                FieldOfView = 8192,
                Position = _resolution.Equals(new(1280.0f, 720.0f)) ? new(0.0f, 0.0f, 869.12f) : new(0.0f, 0.0f, 1000.0f),
                NearPlane = 10.0f,
                FarPlane = 100000.0f
            }
        };

        foreach (var font in csdProject.Fonts)
            result.Fonts.Add(font.ToSrdFont(csdProject.Root));

        foreach (var csdScene in csdProject.Root.Scenes)
            result.Scenes.Add(csdScene.ToSrdScene(result.Fonts));

        for (int i = 0; i < csdProject.Root.Children.Count; i++)
            foreach (var csdScene in csdProject.Root.Children[i].Scenes)
                result.Scenes.Add(csdScene.ToSrdScene(result.Fonts));

        return result;
    }

    public static SurfRide.Draw.Font ToSrdFont(this KeyValuePair<string, Ninja.Csd.Font> csdFont, SceneNode csdRoot)
    {
        SurfRide.Draw.Font result = new()
        {
            Name = csdFont.Key,
            ID = ++_globalID
        };

        for (int i = 0; i < csdFont.Value.Count; i++)
        {
            int cropIndex = 0;

            result.Add(new()
            {
                Code = (ushort)csdFont.Value[i].SourceIndex,
                TextureListIndex = (ushort)(csdRoot.Scenes.Count),
                TextureIndex = (ushort)csdRoot.Scenes[0].Sprites[csdFont.Value[i].DestinationIndex].TextureIndex,
            });

            while (csdRoot.Scenes[0].Sprites[cropIndex].TextureIndex != result[i].TextureIndex)
                cropIndex++;

            result[i].CropIndex = (ushort)(csdFont.Value[i].DestinationIndex - cropIndex);
        }

        return result;
    }

    public static SurfRide.Draw.Scene ToSrdScene(this KeyValuePair<string, Ninja.Csd.Scene> csdScene, List<SurfRide.Draw.Font> srdFonts)
    {
        SurfRide.Draw.Scene srdScene = new()
        {
            Name = csdScene.Key,
            ID = ++_globalID,
            Resolution = ToResolution(csdScene.Value.AspectRatio),
            BackgroundColor = new(68, 68, 204, 255),
            Cameras = new()
            {
                new()
                {
                    Name = "default",
                    ID = ++_globalID,
                    FieldOfView = 8192,
                    Position = _resolution.Equals(new(1280.0f, 720.0f)) ? new(0.0f, 0.0f, 869.12f) : new(0.0f, 0.0f, 1000.0f),
                    NearPlane = 10.0f,
                    FarPlane = 100000.0f
                }
            }
        };

        for (int i = 0; i < csdScene.Value.Families.Count;)
            srdScene.Layers.Add(csdScene.Value.Families[i].ToSrdLayer("Layer_" + (++i).ToString("D3"), srdFonts, srdScene));

        return srdScene;
    }

    public static Layer ToSrdLayer(this Family csdFamily, string name, List<SurfRide.Draw.Font> srdFonts, SurfRide.Draw.Scene srdScene)
    {
        Layer srdLayer = new()
        {
            Name = name,
            ID = ++_globalID,
            Flags = (int)LayerAttribute.Is3D,
            Scene = srdScene
        };

        var nodes = CreateCastTree();

        _index = 0;
        foreach (var csdCast in csdFamily.Casts)
            csdCast.ToSrdCast(nodes, srdLayer, srdFonts);

        foreach (var csdCast in csdFamily)
            ToSrdCell(csdCast, new() { Scale = new(1.0f, 1.0f), Color = new(255, 255, 255, 255) });

        foreach (var motion in csdFamily.Scene.Motions)
            srdLayer.Animations.Add(motion.Value.ToSrdAnimation(motion.Key, csdFamily, srdLayer));

        return srdLayer;

        void ToSrdCell(Cast csdCast, CastInfo csdParentInfo)
        {
            var castPos = csdCast.Info.Translation;
            var castRot = csdCast.Info.Rotation;
            var castScale = csdCast.Info.Scale;
            var castColor = csdCast.Info.Color;
            var cell = srdLayer[csdCast.Priority].Cell;
            var node = srdLayer[csdCast.Priority];

            // Rotate Through Parent Transform
            var angle = csdParentInfo.Rotation.ToRadians();
            var rotatedX = castPos.X * MathF.Cos(angle) * (csdFamily.Scene.AspectRatio) + castPos.Y * MathF.Sin(angle);
            var rotatedY = castPos.Y * MathF.Cos(angle) - castPos.X * MathF.Sin(angle) * (csdFamily.Scene.AspectRatio);

            castPos.X = rotatedX / (csdFamily.Scene.AspectRatio);
            castPos.Y = rotatedY;

            castPos += csdCast.Origin;

            if (csdCast.InheritanceFlags.Test(1))
                castRot += csdParentInfo.Rotation;

            if (csdCast.InheritanceFlags.Test(3))
                node.Flags |= 0x20;

            if (csdCast.InheritanceFlags.Test(8))
                castPos.X += csdParentInfo.Translation.X;

            if (csdCast.InheritanceFlags.Test(9))
                castPos.Y += csdParentInfo.Translation.Y;

            if (csdCast.InheritanceFlags.Test(10))
                castScale.X *= csdParentInfo.Scale.X;

            if (csdCast.InheritanceFlags.Test(11))
                castScale.Y *= csdParentInfo.Scale.Y;
                
            node.Flags |= 0x70000;

            var cellPos = (castPos - csdParentInfo.Translation);
            if (csdCast.Parent == null)
                cellPos -= new Vector2(0.5f, 0.5f);

            cellPos.Y = -cellPos.Y;
            cell.Translation = new(cellPos * _resolution, 0.0f);
            if (node != null && node.Data != null && ((ImageCastData)node.Data).TextData != null && cellPos.X >= 0.0f)
                cell.Translation += new Vector3(5.0f, 0.0f, 0.0f);

            cell.RotationZ = (int)castRot;
            cell.Scale = new(castScale, 1.0f);
            if (_endianness == Endianness.Big)
                cell.MaterialColor = castColor;
            else
                cell.MaterialColor = new(castColor.A, castColor.B, castColor.G, castColor.R);

            CastInfo parentInfo = new()
            {
                Translation = castPos,
                Rotation = castRot,
                Scale = castScale,
                Color = castColor
            };

            foreach (var csdChildCast in csdCast)
                ToSrdCell(csdChildCast, parentInfo);
        }

        Family.TreeDescriptorNode[] CreateCastTree()
        {
            var nodes = new Family.TreeDescriptorNode[csdFamily.Casts.Count];

            // Initialise items because, the runtime doesn't
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = new Family.TreeDescriptorNode();

            BuildTree(csdFamily);

            return nodes;

            void BuildTree(IList<Cast> casts)
            {
                for (int i = 0; i < casts.Count; i++)
                {
                    var cast = casts[i];
                    var idx = csdFamily.Casts.IndexOf(cast);

                    if (cast.Children.Count > 0)
                        nodes[idx].ChildIndex = csdFamily.Casts.IndexOf(cast.Children[0]);

                    if (i + 1 < casts.Count)
                        nodes[idx].SiblingIndex = csdFamily.Casts.IndexOf(casts[i + 1]);

                    BuildTree(cast);
                }
            }
        }
    }

    public static SurfRide.Draw.Animation ToSrdAnimation(this Ninja.Csd.Motions.Motion csdMotion, string name, Family csdFamily, Layer srdLayer)
    {
        var result = new Animation()
        {
            Name = name,
            ID = ++_globalID,
            EndFrame = Convert.ToUInt32(csdMotion.EndFrame),
            Layer = srdLayer
        };

        foreach (var familyMotion in csdMotion.FamilyMotions)
        {
            if (familyMotion.Family != csdFamily)
                continue;

            for (int i = 0; i < familyMotion.CastMotions.Count; i++)
            {
                var node = srdLayer[familyMotion.CastMotions[i].Cast.Priority];

                if (node == null)
                    continue;

                result.Add(new()
                {
                    CastID = (ushort)node.ID,
                });

                foreach (var keyFrameList in familyMotion.CastMotions[i])
                {
                    var curveType = new FCurveType();

                    switch (keyFrameList.Property)
                    {
                        case Ninja.Csd.Motions.KeyProperty.HideFlag:
                            // TODO
                            continue;
                        case Ninja.Csd.Motions.KeyProperty.PositionX:
                            curveType = FCurveType.Tx;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.PositionY:
                            curveType = FCurveType.Ty;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.Rotation:
                            curveType = FCurveType.Rz;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.ScaleX:
                            curveType = FCurveType.Sx;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.ScaleY:
                            curveType = FCurveType.Sy;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.SpriteIndex:
                            // TODO
                            continue;
                        case Ninja.Csd.Motions.KeyProperty.Color:
                            curveType = FCurveType.MaterialColor;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.GradientTopLeft:
                            curveType = FCurveType.VertexColorTopLeft;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.GradientBottomLeft:
                            curveType = FCurveType.VertexColorBottomLeft;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.GradientTopRight:
                            curveType = FCurveType.VertexColorTopRight;
                            break;
                        case Ninja.Csd.Motions.KeyProperty.GradientBottomRight:
                            curveType = FCurveType.VertexColorBottomRight;
                            break;
                        default:
                            break;
                    }

                    Track srdTrack = new()
                    {
                        CurveType = curveType,
                        StartFrame = Convert.ToUInt32(csdMotion.StartFrame),
                        EndFrame = Convert.ToUInt32(csdMotion.EndFrame),
                    };

                    switch (srdTrack.CurveType)
                    {
                        case FCurveType.MaterialColor:
                        case FCurveType.VertexColorTopLeft:
                        case FCurveType.VertexColorTopRight:
                        case FCurveType.VertexColorBottomLeft:
                        case FCurveType.VertexColorBottomRight:
                        case FCurveType.IlluminationColor:
                            srdTrack.Flags |= 0x50;
                            break;
                        default:
                            srdTrack.Flags |= 0x10;
                            break;
                    }

                    foreach (var keyframe in keyFrameList)
                    {
                        SurfRide.Draw.InterpolationType interpolationType = new();

                        switch (keyframe.Interpolation)
                        {
                            case Ninja.Csd.Motions.InterpolationType.Const:
                                interpolationType = InterpolationType.Constant;
                                break;
                            case Ninja.Csd.Motions.InterpolationType.Linear:
                                interpolationType = InterpolationType.Linear;
                                break;
                            case Ninja.Csd.Motions.InterpolationType.Hermite:
                                interpolationType = InterpolationType.Hermite;
                                break;
                            default:
                                break;
                        }

                        srdTrack.Flags |= (uint)interpolationType;

                        var srdKeyframe = new KeyFrame()
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
                            case 0x50:
                                srdKeyframe.Value = new Color<byte>(keyframe.Value.Color.A, keyframe.Value.Color.B, keyframe.Value.Color.G, keyframe.Value.Color.R);
                                break;
                            default:
                                break;
                        }

                        if (srdTrack.CurveType == FCurveType.Tx)
                        {
                            if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin.X == 0.0f)
                                srdKeyframe.Value -= 0.5f;
                            else if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin.X == 1.0f)
                                srdKeyframe.Value += 0.5f;

                            srdKeyframe.Value *= _resolution.X;
                        }
                        else if (srdTrack.CurveType == FCurveType.Ty)
                        {
                            if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin == new Vector2(0.0f, 0.0f))
                                srdKeyframe.Value -= 0.5f;
                            else if (familyMotion.CastMotions[i].Cast.Parent == null &&
                                familyMotion.CastMotions[i].Cast.Origin.X == 1.0f)
                                srdKeyframe.Value += 0.5f;

                            srdKeyframe.Value *= -_resolution.Y;
                        }

                        srdTrack.Add(srdKeyframe);
                    }

                    result[i].Tracks.Add(srdTrack);
                }
            }
        }

        for (int i = result.Count - 1; i >= 0; i--)
            if (result[i].Tracks.Count == 0)
                result.RemoveAt(i);

        return result;
    }

    public static void ToSrdCast(this Cast csdCast, Family.TreeDescriptorNode[] nodeTree, Layer srdLayer, List<SurfRide.Draw.Font> srdFonts)
    {
        srdLayer.Add(new()
        {
            Name = csdCast.Name,
            ID = ++_globalID,
            Flags = csdCast.Field04 != 0 ? 0x4000201 : 0x4000200,
            Data = csdCast.Field04 != 0 ? new ImageCastData() : null,
            ChildIndex = (short)nodeTree[_index].ChildIndex,
            SiblingIndex = (short)nodeTree[_index].SiblingIndex,
            Parent = csdCast.Parent == null ? null : srdLayer[csdCast.Parent.Priority]
        });
        _index++;

        if (srdLayer[csdCast.Priority].Data != null)
        {
            var data = (ImageCastData)srdLayer[csdCast.Priority].Data;

            // Check for Additive blending in Ninja Cell Sprite Draw cast
            if (csdCast.Flags.Test(0))
                data.Flags |= CastAttribute.BlendModeAdd;

            if (csdCast.Flags.Test(10))
                data.Flags |= CastAttribute.FlipHorizontal;

            if (csdCast.Flags.Test(11))
                data.Flags |= CastAttribute.FlipVertical;

            // Check for Linear Filtering in Ninja Cell Sprite Draw cast
            if (csdCast.Flags.Test(12))
                data.Flags |= CastAttribute.AntiAliasing;

            data.Size = (csdCast.BottomRight - csdCast.TopLeft) * _resolution;

            Vector2 topLeft = new(Math.Abs(csdCast.TopLeft.X + csdCast.BottomLeft.X), Math.Abs(csdCast.TopLeft.Y + csdCast.TopRight.Y));
            Vector2 bottomRight = new(Math.Abs(csdCast.TopRight.X + csdCast.BottomRight.X), Math.Abs(csdCast.BottomLeft.Y + csdCast.BottomRight.Y));

            if (topLeft.Y - bottomRight.Y < 0.0001 && topLeft.Y - bottomRight.Y > -0.0001 && topLeft.X - bottomRight.X > 0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionMiddleLeft;
                data.PivotPoint = new(0.0f, data.Size.Y / 2.0f);
            }
            else if (topLeft.Y - bottomRight.Y < 0.0001 && topLeft.Y - bottomRight.Y > -0.0001 && topLeft.X - bottomRight.X < -0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionMiddleRight;
                data.PivotPoint = new(data.Size.X, data.Size.Y / 2.0f);
            }
            else if (topLeft.Y - bottomRight.Y < 0.0001 && topLeft.Y - bottomRight.Y > -0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionMiddleCenter;
                data.PivotPoint = data.Size / 2.0f;
            }
            else if (topLeft.Y - bottomRight.Y > 0.0001 && topLeft.X - bottomRight.X > 0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionTopLeft;
                data.PivotPoint = new(0.0f, 0.0f);
            }
            else if (topLeft.Y - bottomRight.Y > 0.0001 && topLeft.X - bottomRight.X < -0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionTopRight;
                data.PivotPoint = new(data.Size.X, 0.0f);
            }
            else if (topLeft.Y - bottomRight.Y > 0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionTopCenter;
                data.PivotPoint = new(data.Size.X / 2.0f, 0.0f);
            }
            else if (topLeft.Y - bottomRight.Y < -0.0001 && topLeft.X - bottomRight.X > 0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionBottomLeft;
                data.PivotPoint = new(0.0f, data.Size.Y / 2.0f);
            }
            else if (topLeft.Y - bottomRight.Y < -0.0001 && topLeft.X - bottomRight.X < -0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionBottomRight;
                data.PivotPoint = new(data.Size.X, data.Size.Y / 2.0f);
            }
            else if (topLeft.Y - bottomRight.Y < -0.0001)
            {
                data.Flags |= CastAttribute.PivotPositionBottomCenter;
                data.PivotPoint = new(data.Size.X / 2.0f, data.Size.Y);
            }

            data.VertexColorTopLeft = new(csdCast.Info.GradientTopLeft.A, csdCast.Info.GradientTopLeft.B, csdCast.Info.GradientTopLeft.G, csdCast.Info.GradientTopLeft.R);
            data.VertexColorTopRight = new(csdCast.Info.GradientTopRight.A, csdCast.Info.GradientTopRight.B, csdCast.Info.GradientTopRight.G, csdCast.Info.GradientTopRight.R);
            data.VertexColorBottomLeft = new(csdCast.Info.GradientBottomLeft.A, csdCast.Info.GradientBottomLeft.B, csdCast.Info.GradientBottomLeft.G, csdCast.Info.GradientBottomLeft.R);
            data.VertexColorBottomRight = new(csdCast.Info.GradientBottomRight.A, csdCast.Info.GradientBottomRight.B, csdCast.Info.GradientBottomRight.G, csdCast.Info.GradientBottomRight.R);
            data.Surface0.CropIndex = (short)csdCast.Info.SpriteIndex;
            foreach (var spriteIndex in csdCast.SpriteIndices)
            {
                data.Surface0.CropRefs.Add(new()
                {
                    TextureIndex = spriteIndex == -1 ? (short)-1 : (short)csdCast.Family.Scene.Sprites[spriteIndex].TextureIndex,
                    CropIndex = (short)spriteIndex
                });
            }

            data.Surface1.CropIndex = -1;

            if (csdCast.Field04 == 2)
            {
                data.Flags |= CastAttribute.UseFont;

                data.TextData = new()
                {
                    Field00 = 19,
                    Scale = new Vector2(1.0f, 1.0f),
                    Text = csdCast.Text,
                    SpaceCorrection = (short)(csdCast.Field4C * _resolution.X),
                    Field1E = 16
                };

                data.Size += new Vector2(10.0f, 0.0f);

                for (int i = 0; i < srdFonts.Count; i++)
                {
                    if (srdFonts[i].Name == csdCast.FontName)
                    {
                        data.TextData.FontIndex = i;
                        data.TextData.Font = srdFonts[i];
                    }
                }
            }
        }

        srdLayer[csdCast.Priority].Cell = new Cell()
        {
            IlluminationColor = new(0, 0, 0, 255),
            Field08 = (byte)(csdCast.Info.HideFlag == 1 ? 0 : 1),
        };
    }

    unsafe private static Vector2 ToResolution(float aspectRatio)
    {
        var ratio = *((int*)&aspectRatio);
        Vector2 result = ratio == 0x3FAAAAAB ? new(640.0f, 480.0f) : new(1280.0f, 720.0f);

        return result;
    }
}*/