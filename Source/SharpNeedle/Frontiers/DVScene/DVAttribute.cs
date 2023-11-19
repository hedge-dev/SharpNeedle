namespace SharpNeedle.Frontiers.DVScene;

public class DVFadeAttribute : DVNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float[] Values { get; set; } = new float[32];

    public DVFadeAttribute() { }
    public DVFadeAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        reader.ReadArray<float>(32, Values);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.WriteArrayFixedLength(Values, 32);
    }
}

public class DVCullDisabledAttribute : DVNodeData
{
    public DVCullDisabledAttribute() { }
    public DVCullDisabledAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader) { }

    public override void Write(BinaryObjectWriter writer) { }
}

public class DVEffectAttribute : DVNodeData
{
    public override void Read(BinaryObjectReader reader)
    {

    }

    public override void Write(BinaryObjectWriter writer)
    {

    }
}

public class DVLetterboxAttribute : DVNodeData
{
    public float[] Values = new float[32];

    public DVLetterboxAttribute() { }
    public DVLetterboxAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        reader.ReadArray<float>(32, Values);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteArrayFixedLength(Values, 32);
    }
}

public class DVUVAnimAttribute : DVNodeData
{
    public override void Read(BinaryObjectReader reader)
    {

    }

    public override void Write(BinaryObjectWriter writer)
    {

    }
}

public class DVMaterialAnimAttribute : DVNodeData
{
    public override void Read(BinaryObjectReader reader)
    {

    }

    public override void Write(BinaryObjectWriter writer)
    {

    }
}

public class DVChromaAberrAttribute : DVNodeData
{
    public float[] Values = new float[49];

    public DVChromaAberrAttribute() { }
    public DVChromaAberrAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        reader.ReadArray<float>(49, Values);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteArrayFixedLength(Values, 49);
    }
}

public class DVSoundAttribute : DVNodeData
{
    public string CueName;
    public int Field40;
    public int Field44;

    public DVSoundAttribute() { }
    public DVSoundAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        CueName = reader.ReadDVString(64);
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteDVString(CueName);
        writer.Write(Field40);
        writer.Write(Field44);
    }
}

public class DVGameCameraAttribute : DVNodeData
{
    public int Field00;
    public int Field04;
    public int Field08;
    public int Field0C;
    public float Field10;
    public float Field14;
    public float Field18;
    public int Field1C;
    public int Field20;
    public int Field24;
    public float Field28;
    public float Field2C;
    public float Field30;
    public int Field34;
    public int Field38;
    public int Field3C;
    public int Field40;
    public float Field44;
    public int Field48;
    public int Field4C;
    public float NearCullingPlane;
    public float FarCullingPlane;
    public float Field58;
    public int Field5C;
    public int Field60;
    public int Field64;

    public DVGameCameraAttribute() { }
    public DVGameCameraAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<float>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<int>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        NearCullingPlane = reader.Read<float>();
        FarCullingPlane = reader.Read<float>();
        Field58 = reader.Read<float>();
        Field5C = reader.Read<int>();
        Field60 = reader.Read<int>();
        Field64 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
        writer.Write(Field3C);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(NearCullingPlane);
        writer.Write(FarCullingPlane);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);
        writer.Write(Field64);
    }
}

public class DVMovieDisplayAttribute : DVNodeData
{
    public DVMovieDisplayAttribute() { }
    public DVMovieDisplayAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader) { }

    public override void Write(BinaryObjectWriter writer) { }
}

public class DVSubtitleAttribute : DVNodeData
{
    public string CellName { get; set; }
    public SubtitleLanguage Language { get; set; }
    public int Field14 { get; set; }

    public DVSubtitleAttribute() { }
    public DVSubtitleAttribute(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        CellName = reader.ReadDVString(16);
        Language = (SubtitleLanguage)reader.Read<int>();
        Field14 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteDVString(CellName, 16);
        writer.Write((int)Language);
        writer.Write(Field14);
    }
}

public enum SubtitleLanguage
{
    English = 0,
    French = 1,
    Italian = 2,
    German = 3,
    Spanish = 4,
    Polish = 5,
    Portuguese = 6,
    Russian = 7,
    Japanese = 8,
    Chinese1 = 9,
    Chinese2 = 10,
    Korean = 11
}

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
    SpotlightModel = 26,

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
    BossCutoff = 1014,
    Subtitle = 1015,
    Sound = 1016,
    Time = 1017,
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