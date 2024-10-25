namespace SharpNeedle.SonicTeam.DivScene;

public abstract class DivNodeData : IBinarySerializable
{
    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}

public class DivTransformData : DivNodeData
{
    public Matrix4x4 Transform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivTransformData() { }
    public DivTransformData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Transform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Transform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DivCameraData : DivNodeData
{
    public int Field00 { get; set; }
    public int FrameCount { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public List<float> FrameTimes { get; set; } = new List<float>();
    public List<float> FrameData { get; set; } = new List<float>();

    public DivCameraData() { }
    public DivCameraData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        FrameCount = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();

        if(FrameCount > 0)
        {
            reader.ReadCollection(FrameCount, FrameTimes);
            reader.ReadCollection(FrameCount, FrameData);
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(FrameCount);
        writer.Write(Field08);
        writer.Write(Field0C);

        if (FrameCount > 0)
        {
            writer.WriteCollection(FrameTimes);
            writer.WriteCollection(FrameData);
        }
    }
}

public class DivCameraMotionData : DivNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DivCameraMotionData() { }
    public DivCameraMotionData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
    }
}

public class DivModelData : DivNodeData
{
    public int Field00 { get; set; }
    public string ModelName { get; set; }
    public string SkeletonName { get; set; }
    public string Field84 { get; set; }

    public DivModelData() { }
    public DivModelData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        ModelName = reader.ReadDivString(64);
        SkeletonName = reader.ReadDivString(64);
        Field84 = reader.ReadDivString(64);

        reader.Skip(76);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteDivString(ModelName, 64);
        writer.WriteDivString(SkeletonName, 64);
        writer.WriteDivString(Field84, 64);

        writer.WriteNulls(76);
    }
}

public class DivMotionData : DivNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public string Field10 { get; set; }
    public float Field18 { get; set; }

    public DivMotionData() { }
    public DivMotionData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.ReadDivString(8);
        Field18 = reader.Read<float>();

        reader.Skip(20);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.WriteDivString(Field10, 8);
        writer.Write(Field18);

        writer.WriteNulls(20);
    }
}

public class DivNodeAttachData : DivNodeData
{
    public int Field00 { get; set; }
    public string NodeName { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivNodeAttachData() { }
    public DivNodeAttachData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        NodeName = reader.ReadDivString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteDivString(NodeName, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DivParameterData : DivNodeData
{
    private int UnknownDataSize;

    public int Type { get; set; }
    public float StartTime { get; set; }
    public float EndTime { get; set; }
    public int Field0C { get; set; }
    public int Field10 { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public DivParameter Parameter { get; set; }
    
    public DivParameterData() { }
    public DivParameterData(BinaryObjectReader reader, int size)
    {
        UnknownDataSize = size - 8;
        Read(reader);
    }

    public DivParameterData(ParameterType type, float startTime, float endTime)
    {
        Type = (int)type;
        StartTime = startTime;
        EndTime = endTime;
    }

    public DivParameterData(ParameterType type, float startTime, float endTime, DivParameter parameter) : this(type, startTime, endTime)
    {
        Parameter = parameter;
    }

    public void ReadFrontiersParameter(BinaryObjectReader reader, int type)
    {
        switch ((FrontiersParams)type)
        {
            case FrontiersParams.DepthOfField:
                Parameter = new DivDOFParameter(reader);
                break;

            case FrontiersParams.ColorCorrection:
                Parameter = new DivColorCorrectionParameter(reader);
                break;

            case FrontiersParams.CameraExposure:
                Parameter = new DivCameraExposureParameter(reader);
                break;

            case FrontiersParams.ShadowResolution:
                Parameter = new DivShadowResolutionParameter(reader);
                break;

            case FrontiersParams.ChromaticAberration:
                Parameter = new DivChromaAberrParameter(reader);
                break;

            case FrontiersParams.Vignette:
                Parameter = new DivVignetteParameter(reader);
                break;

            case FrontiersParams.Fade:
                Parameter = new DivFadeParameter(reader);
                break;

            case FrontiersParams.Letterbox:
                Parameter = new DivLetterboxParameter(reader);
                break;

            case FrontiersParams.BossCutoff:
                Parameter = new DivBossCutoffParameter(reader);
                break;

            case FrontiersParams.Subtitle:
                Parameter = new DivSubtitleParameter(reader);
                break;

            case FrontiersParams.Sound:
                Parameter = new DivSoundParameter(reader);
                break;

            case FrontiersParams.Time:
                Parameter = new DivTimeParameter(reader);
                break;

            case FrontiersParams.CameraBlur:
                Parameter = new DivCameraBlurParameter(reader);
                break;

            case FrontiersParams.GeneralPurposeTrigger:
                Parameter = new DivGeneralPurposeTriggerParameter(reader);
                break;

            case FrontiersParams.DitherDepth:
                Parameter = new DivDitherDepthParameter(reader);
                break;

            case FrontiersParams.QTE:
                Parameter = new DivQTEParameter(reader);
                break;

            case FrontiersParams.ASMForcedOverwrite:
                Parameter = new DivASMParameter(reader);
                break;

            case FrontiersParams.Aura:
                Parameter = new DivAuraParameter(reader);
                break;

            case FrontiersParams.TimescaleChange:
                Parameter = new DivTimescaleParameter(reader);
                break;

            case FrontiersParams.CyberNoise:
                Parameter = new DivCyberNoiseParameter(reader);
                break;

            case FrontiersParams.MovieDisplay:
                Parameter = new DivMovieDisplayParameter(reader);
                break;

            case FrontiersParams.Weather:
                Parameter = new DivWeatherParameter(reader);
                break;

            case FrontiersParams.TheEndCable:
                Parameter = new DivTheEndCableParameter(reader);
                break;

            case FrontiersParams.FinalBossLighting:
                Parameter = new DivFinalBossLightingParameter(reader);
                break;

            default:
                Parameter = new DivUnknownParameter(reader, UnknownDataSize);
                break;
        }
    }

    public void ReadShadowGensParameter(BinaryObjectReader reader, int type)
    {
        switch ((ShadowGensParams)type)
        {
            case ShadowGensParams.DepthOfField:
                Parameter = new DivDOFParameter(reader);
                break;

            case ShadowGensParams.ChromaticAberration:
                Parameter = new DivChromaAberrParameter(reader);
                break;

            case ShadowGensParams.Vignette:
                Parameter = new DivVignetteParameter(reader);
                break;

            case ShadowGensParams.Fade:
                Parameter = new DivFadeParameter(reader);
                break;

            case ShadowGensParams.BossCutoff:
                Parameter = new DivBossCutoffParameter(reader);
                break;

            case ShadowGensParams.Subtitle:
                Parameter = new DivSubtitleParameter(reader);
                break;

            case ShadowGensParams.Sound:
                Parameter = new DivSoundParameter(reader);
                break;

            case ShadowGensParams.CameraBlur:
                Parameter = new DivCameraBlurParameter(reader);
                break;

            case ShadowGensParams.GeneralPurposeTrigger:
                Parameter = new DivGeneralPurposeTriggerParameter(reader);
                break;

            case ShadowGensParams.QTE:
                Parameter = new DivQTEParameter(reader);
                break;

            case ShadowGensParams.TimescaleChange:
                Parameter = new DivTimescaleParameter(reader);
                break;

            default:
                Parameter = new DivUnknownParameter(reader, UnknownDataSize);
                break;
        }
    }

    public void ReadGameSpecificParameter(BinaryObjectReader reader, GameType game, int type)
    {
        switch(game)
        {
            case GameType.Frontiers:
                ReadFrontiersParameter(reader, type);
                break;

            case GameType.ShadowGenerations:
                ReadShadowGensParameter(reader, type);
                break;

            default:
                Parameter = new DivUnknownParameter(reader, UnknownDataSize);
                break;
        }
    }

    public override void Read(BinaryObjectReader reader)
    {
        Type = reader.Read<int>();
        StartTime = reader.Read<float>();
        EndTime = reader.Read<float>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<int>();
        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();

        if(Type < 1000)
        {
            switch ((ParameterType)Type)
            {
                case ParameterType.DrawingOff:
                    Parameter = new DivDrawingOffParameter(reader);
                    break;

                case ParameterType.PathAdjust:
                    Parameter = new DivPathAdjustParameter(reader);
                    break;

                case ParameterType.Effect:
                    Parameter = new DivEffectParameter(reader);
                    break;

                case ParameterType.CullDisabled:
                    Parameter = new DivCullDisabledParameter(reader);
                    break;

                case ParameterType.UVAnimation:
                    Parameter = new DivUVAnimParameter(reader);
                    break;

                case ParameterType.MaterialAnimation:
                    Parameter = new DivMaterialAnimParameter(reader);
                    break;

                case ParameterType.CompositeAnimation:
                    Parameter = new DivCompositeAnimationParameter(reader);
                    break;

                case ParameterType.GameCamera:
                    Parameter = new DivGameCameraParameter(reader);
                    break;

                default:
                    Parameter = new DivUnknownParameter(reader, UnknownDataSize);
                    break;
            }
        } 
        else
        {
            ReadGameSpecificParameter(reader, GameType.Frontiers, Type);
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Type);
        writer.Write(StartTime);
        writer.Write(EndTime);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        
        if(Parameter != null)
            Parameter.Write(writer);
    }
}

public enum NodeType
{
    Transform = 1,
    Camera = 3,
    CameraMotion = 4,
    Model = 5,
    Motion = 6,
    Model2 = 8,
    Motion2 = 10,
    NodeAttachment = 11,
    Parameter = 12
}