namespace SharpNeedle.Framework.SonicTeam.DiEvent.NodeData;

using SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class ParameterData : BaseNodeData
{
    private readonly int _unknownDataSize;

    public float StartTime { get; set; }
    public float EndTime { get; set; }
    public int Field0C { get; set; }
    public int Field10 { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public BaseParam? Parameter { get; set; }

    public ParameterData() { }

    public ParameterData(int dataSize)
    {
        _unknownDataSize = dataSize - 8;
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

    private static Func<GameType, BaseParam>? GetCommonReadFunc(BinaryObjectReader reader, int type)
    {
        return (ParameterType)type switch
        {
            ParameterType.DrawingOff => reader.ReadObject<DrawOffParam, GameType>,
            ParameterType.PathAdjust => reader.ReadObject<PathAdjustParam, GameType>,
            ParameterType.CameraShake => reader.ReadObject<CameraShakeParam, GameType>,
            ParameterType.CameraShakeLoop => reader.ReadObject<CameraShakeLoopParam, GameType>,
            ParameterType.Effect => reader.ReadObject<EffectParam, GameType>,
            ParameterType.CullDisabled => reader.ReadObject<CullDisabledParam, GameType>,
            ParameterType.UVAnimation => reader.ReadObject<UVAnimParam, GameType>,
            ParameterType.VisibilityAnimation => reader.ReadObject<VisibilityAnimParam, GameType>,
            ParameterType.MaterialAnimation => reader.ReadObject<MaterialAnimParam, GameType>,
            ParameterType.CompositeAnimation => reader.ReadObject<CompositeAnimParam, GameType>,
            ParameterType.SonicCamera => reader.ReadObject<SonicCameraParam, GameType>,
            ParameterType.GameCamera => reader.ReadObject<GameCameraParam, GameType>,
            ParameterType.ControllerVibration => reader.ReadObject<ControllerVibrationParam, GameType>,
            ParameterType.MaterialParameter => reader.ReadObject<MaterialParameterParam, GameType>,
            _ => null,
        };
    }

    private static Func<GameType, BaseParam>? GetFrontiersReadFunc(BinaryObjectReader reader, int type)
    {
        return (FrontiersParams)type switch
        {
            FrontiersParams.DepthOfField => reader.ReadObject<DOFParam, GameType>,
            FrontiersParams.ColorCorrection => reader.ReadObject<ColorCorrectionParam, GameType>,
            FrontiersParams.CameraExposure => reader.ReadObject<CameraExposureParam, GameType>,
            FrontiersParams.ShadowResolution => reader.ReadObject<ShadowResolutionParam, GameType>,
            FrontiersParams.ChromaticAberration => reader.ReadObject<ChromaAberrationParam, GameType>,
            FrontiersParams.Vignette => reader.ReadObject<VignetteParam, GameType>,
            FrontiersParams.Fade => reader.ReadObject<FadeParam, GameType>,
            FrontiersParams.Letterbox => reader.ReadObject<LetterboxParam, GameType>,
            FrontiersParams.BossName => reader.ReadObject<BossNameParam, GameType>,
            FrontiersParams.Subtitle => reader.ReadObject<SubtitleParam, GameType>,
            FrontiersParams.Sound => reader.ReadObject<SoundParam, GameType>,
            FrontiersParams.Time => reader.ReadObject<TimeParam, GameType>,
            FrontiersParams.CameraBlur => reader.ReadObject<CameraBlurParam, GameType>,
            FrontiersParams.GeneralPurposeTrigger => reader.ReadObject<GeneralTriggerParam, GameType>,
            FrontiersParams.DitherDepth => reader.ReadObject<DitherDepthParam, GameType>,
            FrontiersParams.QTE => reader.ReadObject<QTEParam, GameType>,
            FrontiersParams.ASMForcedOverwrite => reader.ReadObject<ASMOverrideParam, GameType>,
            FrontiersParams.Aura => reader.ReadObject<AuraParam, GameType>,
            FrontiersParams.TimescaleChange => reader.ReadObject<TimescaleParam, GameType>,
            FrontiersParams.CyberNoise => reader.ReadObject<CyberNoiseParam, GameType>,
            FrontiersParams.MovieDisplay => reader.ReadObject<MovieDisplayParam, GameType>,
            FrontiersParams.Weather => reader.ReadObject<WeatherParam, GameType>,
            FrontiersParams.TheEndCable => reader.ReadObject<TheEndCableParam, GameType>,
            FrontiersParams.FinalBossLighting => reader.ReadObject<FinalBossLightingParam, GameType>,
            _ => null,
        };
    }

    private static Func<GameType, BaseParam>? GetShadowGenerationsReadFunc(BinaryObjectReader reader, int type)
    {
        return (ShadowGensParams)type switch
        {
            ShadowGensParams.DepthOfField => reader.ReadObject<DOFParam, GameType>,
            ShadowGensParams.Fade => reader.ReadObject<FadeParam, GameType>,
            ShadowGensParams.BossName => reader.ReadObject<BossNameParam, GameType>,
            ShadowGensParams.Subtitle => reader.ReadObject<SubtitleParam, GameType>,
            ShadowGensParams.Sound => reader.ReadObject<SoundParam, GameType>,
            ShadowGensParams.GeneralPurposeTrigger => reader.ReadObject<GeneralTriggerParam, GameType>,
            ShadowGensParams.QTE => reader.ReadObject<QTEParam, GameType>,
            ShadowGensParams.MovieDisplay => reader.ReadObject<MovieDisplayParam, GameType>,
            ShadowGensParams.TimeStop => reader.ReadObject<TimeStopParam, GameType>,
            ShadowGensParams.TimeStopControl => reader.ReadObject<TimeStopControlParam, GameType>,
            ShadowGensParams.TimeStopObjectBehavior => reader.ReadObject<TimeStopObjectBehaviorParam, GameType>,
            ShadowGensParams.ShadowAfterimage => reader.ReadObject<ShadowAfterimageParam, GameType>,
            ShadowGensParams.FalloffToggle => reader.ReadObject<FalloffToggleParam, GameType>,
            _ => null,
        };
    }

    private BaseParam ReadParameter(BinaryObjectReader reader, GameType game, int type)
    {
        Func<GameType, BaseParam>? readFunc;

        if(type < 1000)
        {
            game = GameType.Common;
        }

        readFunc = game switch
        {
            GameType.Frontiers => GetFrontiersReadFunc(reader, type),
            GameType.ShadowGenerations => GetShadowGenerationsReadFunc(reader, type),
            _ => GetCommonReadFunc(reader, type),
        };
        

        if(readFunc == null)
        {
            UnknownParam result = new(_unknownDataSize, type);
            result.Read(reader, game);
            return result;
        }
        else
        {
            return readFunc(game);
        }
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        int type = reader.ReadInt32();
        StartTime = reader.ReadSingle();
        EndTime = reader.ReadSingle();
        Field0C = reader.ReadInt32();
        Field10 = reader.ReadInt32();
        Field14 = reader.ReadInt32();
        Field18 = reader.ReadInt32();
        Field1C = reader.ReadInt32();
        Parameter = ReadParameter(reader, game, type);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        int type = Parameter == null ? 0 : Parameter.GetTypeID(game);

        writer.WriteInt32(type);
        writer.WriteSingle(StartTime);
        writer.WriteSingle(EndTime);
        writer.WriteInt32(Field0C);
        writer.WriteInt32(Field10);
        writer.WriteInt32(Field14);
        writer.WriteInt32(Field18);
        writer.WriteInt32(Field1C);

        if (Parameter != null)
        {
            writer.WriteObject(Parameter, game);
        }
    }
}