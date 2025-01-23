namespace SharpNeedle.Framework.SonicTeam.DiEvent;

using SharpNeedle.Structs;

public abstract class BaseParam : IBinarySerializable<GameType>
{
    public virtual void Read(BinaryObjectReader reader, GameType game) { }

    public virtual void Write(BinaryObjectWriter reader, GameType game) { }

    public virtual int GetTypeID(GameType game) { return 0; }
}

public class LetterboxParam : BaseParam
{
    public float[] CurveData { get; set; } = new float[32];

    public LetterboxParam() { }
    public LetterboxParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Letterbox;

            default:
                return 0;
        }
    }
}

public class ASMOverrideParam : BaseParam
{
    public string OverriddenASMName { get; set; } = string.Empty;
    public string OverridingASMName { get; set; } = string.Empty;

    public ASMOverrideParam() { }

    public ASMOverrideParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        OverriddenASMName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        OverridingASMName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, OverriddenASMName, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, OverridingASMName, 64);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ASMForcedOverwrite;

            default:
                return 0;
        }
    }
}

public class GeneralTriggerParam : BaseParam
{
    public uint Field00 { get; set; }
    public string TriggerName { get; set; } = string.Empty;

    public GeneralTriggerParam() { }

    public GeneralTriggerParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        TriggerName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, TriggerName, 64);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.GeneralPurposeTrigger;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.GeneralPurposeTrigger;

            default:
                return 0;
        }
    }
}

public class FinalBossLightingParam : BaseParam
{
    public FinalBossLightingParam() { }
    public FinalBossLightingParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game) { }

    public override void Write(BinaryObjectWriter writer, GameType game) { }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.FinalBossLighting;

            default:
                return 0;
        }
    }
}

public class DitherDepthParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }

    public DitherDepthParam() { }
    public DitherDepthParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.DitherDepth;

            default:
                return 0;
        }
    }
}

// < 1000 values are shared between games, the others are game-specific.
public enum ParameterType
{
    ParameterSpecifiedCamera = 1,
    DrawingOff = 3,
    PathAdjust = 5,
    CameraShake = 6,
    CameraShakeLoop = 7,
    Effect = 8,
    PathInterpolation = 10,
    CullDisabled = 11,
    NearFarSetting = 12,
    UVAnimation = 13,
    VisibilityAnimation = 14,
    MaterialAnimation = 15,
    CompositeAnimation = 16,
    CameraOffset = 17,
    ModelFade = 18,
    SonicCamera = 20,
    GameCamera = 21,
    PointLightSource = 22,
    ControllerVibration = 25,
    SpotlightModel = 26,
    MaterialParameter = 27,
}

enum FrontiersParams
{
    DepthOfField = 1001,
    ColorCorrection = 1002,
    CameraExposure = 1003,
    ShadowResolution = 1004,
    HeightFog = 1007,
    ChromaticAberration = 1008,
    Vignette = 1009,
    Fade = 1010,
    Letterbox = 1011,
    ModelClipping = 1012,
    BossName = 1014,
    Subtitle = 1015,
    Sound = 1016,
    Time = 1017,
    LookAtIK = 1019,
    CameraBlur = 1020,
    GeneralPurposeTrigger = 1021,
    DitherDepth = 1023,
    QTE = 1024,
    FacialAnimation = 1025,
    ASMForcedOverwrite = 1026,
    Aura = 1027,
    TimescaleChange = 1028,
    CyberNoise = 1029,
    AuraRoad = 1031,
    MovieDisplay = 1032,
    Weather = 1034,
    PointLightSource = 1036,
    TheEndCable = 1042,
    FinalBossLighting = 1043
}

enum ShadowGensParams
{
    Bloom = 1000,
    DepthOfField = 1001,
    HeightFog = 1009,
    ChromaticAberration = 1010,
    Vignette = 1011,
    Fade = 1012,
    BossName = 1016,
    Subtitle = 1017,
    Sound = 1018,
    CameraBlur = 1022,
    GeneralPurposeTrigger = 1023,
    QTE = 1026,
    TimescaleChange = 1030,
    MovieDisplay = 1034,
    TimeStop = 1041,
    TimeStopControl = 1042,
    TimeStopObjectBehavior = 1043,
    ShadowAfterimage = 1044,
    FalloffToggle = 1045,
    Fog = 1046,
    DepthOfFieldNew = 1047,
}