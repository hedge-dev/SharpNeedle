namespace SharpNeedle.Framework.SonicTeam.DiEvent;

public abstract class BaseNodeData : IBinarySerializable<GameType>
{
    public virtual void Read(BinaryObjectReader reader, GameType type) { }

    public virtual void Write(BinaryObjectWriter writer, GameType game) { }
}

public class PathData : BaseNodeData
{
    public Matrix4x4 Transform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public PathData() { }
    public PathData(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Transform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Transform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class CameraData : BaseNodeData
{
    public int Field00 { get; set; }
    public int FrameCount { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public List<float> FrameTimes { get; set; } = [];
    public List<float> FrameData { get; set; } = [];

    public CameraData() { }
    public CameraData(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
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

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(FrameCount);
        writer.Write(Field08);
        writer.Write(Field0C);

        if(FrameCount > 0)
        {
            writer.WriteCollection(FrameTimes);
            writer.WriteCollection(FrameData);
        }
    }
}

public class CameraMotionData : BaseNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public CameraMotionData() { }
    public CameraMotionData(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
    }
}

public class ModelData : BaseNodeData
{
    public int Field00 { get; set; }
    public string ModelName { get; set; }
    public string SkeletonName { get; set; }
    public string Field84 { get; set; }

    public ModelData() { }
    public ModelData(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        ModelName = reader.ReadDiString(64);
        SkeletonName = reader.ReadDiString(64);
        Field84 = reader.ReadDiString(64);

        reader.Skip(76);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(ModelName, 64);
        writer.WriteDiString(SkeletonName, 64);
        writer.WriteDiString(Field84, 64);

        writer.WriteNulls(76);
    }
}

public class MotionModelData : BaseNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public string Field10 { get; set; }
    public float Field18 { get; set; }

    public MotionModelData() { }
    public MotionModelData(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.ReadDiString(8);
        Field18 = reader.Read<float>();

        reader.Skip(20);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.WriteDiString(Field10, 8);
        writer.Write(Field18);

        writer.WriteNulls(20);
    }
}

public class AttachmentData : BaseNodeData
{
    public int Field00 { get; set; }
    public string NodeName { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public AttachmentData() { }
    public AttachmentData(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        NodeName = reader.ReadDiString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(NodeName, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class ParameterData : BaseNodeData
{
    private int UnknownDataSize;

    public float StartTime { get; set; }
    public float EndTime { get; set; }
    public int Field0C { get; set; }
    public int Field10 { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public BaseParam Parameter { get; set; }

    public ParameterData() { }
    public ParameterData(BinaryObjectReader reader, GameType game, int size)
    {
        UnknownDataSize = size - 8;
        Read(reader, game);
    }

    public ParameterData(float startTime, float endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public ParameterData(float startTime, float endTime, BaseParam parameter) : this(startTime, endTime)
    {
        Parameter = parameter;
    }

    public void ReadFrontiersParameter(BinaryObjectReader reader, int type)
    {
        switch((FrontiersParams)type)
        {
            case FrontiersParams.DepthOfField:
                Parameter = new DOFParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ColorCorrection:
                Parameter = new ColorCorrectionParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.CameraExposure:
                Parameter = new CameraExposureParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ShadowResolution:
                Parameter = new ShadowResolutionParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ChromaticAberration:
                Parameter = new ChromaAberrationParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Vignette:
                Parameter = new VignetteParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Fade:
                Parameter = new FadeParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Letterbox:
                Parameter = new LetterboxParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.BossName:
                Parameter = new BossNameParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Subtitle:
                Parameter = new SubtitleParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Sound:
                Parameter = new SoundParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Time:
                Parameter = new TimeParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.CameraBlur:
                Parameter = new CameraBlurParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.GeneralPurposeTrigger:
                Parameter = new GeneralTriggerParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.DitherDepth:
                Parameter = new DitherDepthParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.QTE:
                Parameter = new QTEParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ASMForcedOverwrite:
                Parameter = new ASMOverrideParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Aura:
                Parameter = new AuraParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.TimescaleChange:
                Parameter = new TimescaleParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.CyberNoise:
                Parameter = new CyberNoiseParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.MovieDisplay:
                Parameter = new MovieDisplayParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Weather:
                Parameter = new WeatherParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.TheEndCable:
                Parameter = new TheEndCableParam(reader, GameType.Frontiers);
                break;

            case FrontiersParams.FinalBossLighting:
                Parameter = new FinalBossLightingParam(reader, GameType.Frontiers);
                break;

            default:
                Parameter = new UnknownParam(reader, UnknownDataSize, type);
                break;
        }
    }

    public void ReadShadowGensParameter(BinaryObjectReader reader, int type)
    {
        switch((ShadowGensParams)type)
        {
            case ShadowGensParams.DepthOfField:
                Parameter = new DOFParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.Fade:
                Parameter = new FadeParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.BossName:
                Parameter = new BossNameParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.Subtitle:
                Parameter = new SubtitleParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.Sound:
                Parameter = new SoundParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.GeneralPurposeTrigger:
                Parameter = new GeneralTriggerParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.QTE:
                Parameter = new QTEParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.MovieDisplay:
                Parameter = new MovieDisplayParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.TimeStop:
                Parameter = new TimeStopParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.TimeStopControl:
                Parameter = new TimeStopControlParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.TimeStopObjectBehavior:
                Parameter = new TimeStopObjectBehaviorParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.ShadowAfterimage:
                Parameter = new ShadowAfterimageParam(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.FalloffToggle:
                Parameter = new FalloffToggleParam(reader, GameType.ShadowGenerations);
                break;

            default:
                Parameter = new UnknownParam(reader, UnknownDataSize, type);
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
                Parameter = new UnknownParam(reader, UnknownDataSize, type);
                break;
        }
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        int type = reader.Read<int>();
        StartTime = reader.Read<float>();
        EndTime = reader.Read<float>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<int>();
        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();

        if(type < 1000)
        {
            switch((ParameterType)type)
            {
                case ParameterType.DrawingOff:
                    Parameter = new DrawOffParam(reader, GameType.Common);
                    break;

                case ParameterType.PathAdjust:
                    Parameter = new PathAdjustParam(reader, GameType.Common);
                    break;

                case ParameterType.CameraShake:
                    Parameter = new CameraShakeParam(reader, GameType.Common);
                    break;

                case ParameterType.CameraShakeLoop:
                    Parameter = new CameraShakeLoopParam(reader, GameType.Common);
                    break;

                case ParameterType.Effect:
                    Parameter = new EffectParam(reader, GameType.Common);
                    break;

                case ParameterType.CullDisabled:
                    Parameter = new CullDisabledParam(reader, GameType.Common);
                    break;

                case ParameterType.UVAnimation:
                    Parameter = new UVAnimParam(reader, GameType.Common);
                    break;

                case ParameterType.VisibilityAnimation:
                    Parameter = new VisibilityAnimParam(reader, GameType.Common);
                    break;

                case ParameterType.MaterialAnimation:
                    Parameter = new MaterialAnimParam(reader, GameType.Common);
                    break;

                case ParameterType.CompositeAnimation:
                    Parameter = new CompositeAnimParam(reader, GameType.Common);
                    break;

                case ParameterType.SonicCamera:
                    Parameter = new SonicCameraParam(reader, GameType.Common);
                    break;

                case ParameterType.GameCamera:
                    Parameter = new GameCameraParam(reader, GameType.Common);
                    break;

                case ParameterType.ControllerVibration:
                    Parameter = new ControllerVibrationParam(reader, GameType.Common);
                    break;

                case ParameterType.MaterialParameter:
                    Parameter = new MaterialParameterParam(reader, GameType.Common);
                    break;

                default:
                    Parameter = new UnknownParam(reader, UnknownDataSize, type);
                    break;
            }
        }
        else
        {
            ReadGameSpecificParameter(reader, game, type);
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        int type = Parameter == null ? 0 : Parameter.GetTypeID(game);

        writer.Write(type);
        writer.Write(StartTime);
        writer.Write(EndTime);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);

        if(Parameter != null)
        {
            writer.WriteObject(Parameter, game);
        }
    }
}

public enum NodeType
{
    Root = 0,
    Path = 1,
    PathMotion = 2,
    Camera = 3,
    CameraMotion = 4,
    Character = 5,
    CharacterMotion = 6,
    CharacterBehavior = 7,
    ModelCustom = 8,
    Asset = 9,
    ModelMotion = 10,
    Attachment = 11,
    Parameter = 12,
    Stage = 13,
    StageScenarioFlag = 14,
    InstanceMotion = 15,
    InstanceMotionData = 16,
    FolderCondition = 17,
    CharacterBehaviorSimpleTalk = 18,
}