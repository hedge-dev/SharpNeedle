namespace SharpNeedle.SonicTeam.DiEvent;

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
        switch ((FrontiersParams)type)
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
        switch ((ShadowGensParams)type)
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
        switch (game)
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

        if (type < 1000)
        {
            switch ((ParameterType)type)
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
        int type = (Parameter == null) ? 0 : Parameter.GetTypeID(game);

        writer.Write(type);
        writer.Write(StartTime);
        writer.Write(EndTime);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);

        if (Parameter != null)
            writer.WriteObject(Parameter, game);
    }
}