namespace SharpNeedle.Frontiers.DVScene;

public abstract class DVParameter : IBinarySerializable
{
    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}

public class DVCullDisabledParameter : DVParameter
{
    public DVCullDisabledParameter() { }
    public DVCullDisabledParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader) { }

    public override void Write(BinaryObjectWriter writer) { }
}

public class DVMovieDisplayParameter : DVParameter
{
    public DVMovieDisplayParameter() { }
    public DVMovieDisplayParameter(BinaryObjectReader reader) { }

    public override void Read(BinaryObjectReader reader) { }
    public override void Write(BinaryObjectWriter writer) { }
}

public class DVUnknownParameter : DVParameter
{
    public uint[] Data { get; set; }
    public int Size { get; set; }

    public DVUnknownParameter() { }
    public DVUnknownParameter(BinaryObjectReader reader, int size) 
    {
        Size = size;
        Data = new uint[Size];

        Read(reader);
    }

    public override void Read(BinaryObjectReader reader) 
    {
        reader.ReadArray<uint>(Size, Data);
    }

    public override void Write(BinaryObjectWriter writer) 
    {
        writer.WriteArrayFixedLength(Data, Size);
    }
}

class DVDrawingOffParameter : DVParameter
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DVDrawingOffParameter() { }
    public DVDrawingOffParameter(BinaryObjectReader reader)
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

public class DVFadeParameter : DVParameter
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float[] Values { get; set; } = new float[32];

    public DVFadeParameter() { }
    public DVFadeParameter(BinaryObjectReader reader)
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

public class DVEffectParameter : DVParameter
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public string Name { get; set; }
    public int Field84 { get; set; }
    public int Field88 { get; set; }
    public int Field8C { get; set; }
    public int Field90 { get; set; }
    public int Field94 { get; set; }
    public int Field98 { get; set; }
    public int Field9C { get; set; }
    public int FieldA0 { get; set; }
    public float[] FieldA4 { get; set; } = new float[128];

    public DVEffectParameter() { }
    public DVEffectParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Name = reader.ReadDVString(64);
        Field84 = reader.Read<int>();
        Field88 = reader.Read<int>();
        Field8C = reader.Read<int>();
        Field90 = reader.Read<int>();
        Field94 = reader.Read<int>();
        Field98 = reader.Read<int>();
        Field9C = reader.Read<int>();
        FieldA0 = reader.Read<int>();
        reader.ReadArray<float>(128, FieldA4);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.WriteDVString(Name);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
        writer.Write(Field90);
        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(FieldA0);
        writer.WriteArrayFixedLength(FieldA4, 128);
    }
}

public class DVLetterboxParameter : DVParameter
{
    public float[] Values { get; set; } = new float[32];

    public DVLetterboxParameter() { }
    public DVLetterboxParameter(BinaryObjectReader reader)
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

public class DVUVAnimParameter : DVParameter
{
    public int Field00 { get; set; }
    public string Name { get; set; }
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public DVUVAnimParameter() { }
    public DVUVAnimParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDVString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteDVString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }
}

public class DVMaterialAnimParameter : DVParameter
{
    public int Field00 { get; set; }
    public string Name { get; set; }
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public DVMaterialAnimParameter() { }
    public DVMaterialAnimParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDVString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteDVString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }
}

public class DVChromaAberrParameter : DVParameter
{
    public float[] Values = new float[49];

    public DVChromaAberrParameter() { }
    public DVChromaAberrParameter(BinaryObjectReader reader)
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

public class DVSoundParameter : DVParameter
{
    public string CueName { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }

    public DVSoundParameter() { }
    public DVSoundParameter(BinaryObjectReader reader)
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

public class DVGameCameraParameter : DVParameter
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public float Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public float NearCullingPlane { get; set; }
    public float FarCullingPlane { get; set; }
    public float Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public int Field64 { get; set; }

    public DVGameCameraParameter() { }
    public DVGameCameraParameter(BinaryObjectReader reader)
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

public class DVSubtitleParameter : DVParameter
{
    public string CellName { get; set; }
    public SubtitleLanguage Language { get; set; }
    public int Field14 { get; set; }

    public DVSubtitleParameter() { }
    public DVSubtitleParameter(BinaryObjectReader reader)
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
}

public class DVQTEParameter : DVParameter
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public int Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public int Field64 { get; set; }
    public int Field68 { get; set; }
    public int Field6C { get; set; }
    public float Field70 { get; set; }
    public float Field74 { get; set; }
    public float Field78 { get; set; }
    public float Field7C { get; set; }
    public int[] Field80 { get; set; } = new int[48];
    public string Field140 { get; set; }

    public DVQTEParameter() { }
    public DVQTEParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<int>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
        Field54 = reader.Read<int>();
        Field58 = reader.Read<int>();
        Field5C = reader.Read<int>();
        Field60 = reader.Read<int>();
        Field64 = reader.Read<int>();
        Field68 = reader.Read<int>();
        Field6C = reader.Read<int>();
        Field70 = reader.Read<float>();
        Field74 = reader.Read<float>();
        Field78 = reader.Read<float>();
        Field7C = reader.Read<float>();
        reader.ReadArray<int>(48, Field80);

        Field140 = reader.ReadDVString(64);
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
        writer.Write(Field50);
        writer.Write(Field54);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);
        writer.Write(Field64);
        writer.Write(Field68);
        writer.Write(Field6C);
        writer.Write(Field70);
        writer.Write(Field74);
        writer.Write(Field78);
        writer.Write(Field7C);
        writer.WriteArrayFixedLength(Field80, 48);

        writer.WriteDVString(Field140, 64);
    }
}

public class DVTimescaleParameter : DVParameter
{
    public int Field00 { get; set; }
    public float Scale { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DVTimescaleParameter() { }
    public DVTimescaleParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Scale = reader.Read<float>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Scale);
        writer.Write(Field08);
        writer.Write(Field0C);
    }
}
/*
public class DVVignetteParameter : DVParameter
{
    public int Field00;
    public int Field04;
    public float Field08;
    public float Field0C;

    public float[] FieldC8 = new float[32];

    public DVVignetteParameter() { }
    public DVVignetteParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();

        reader.ReadArray<float>(32, FieldC8);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);

        writer.WriteArrayFixedLength(FieldC8, 32);
    }
}
*/

public class DVPathAdjustParameter : DVParameter
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DVPathAdjustParameter() { }
    public DVPathAdjustParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DVBossCutoffParameter : DVParameter
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }

    public DVBossCutoffParameter() { }
    public DVBossCutoffParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
    }
}

public class DVAuraParameter : DVParameter
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public float[] Values { get; set; } = new float[32];

    public DVAuraParameter() { }
    public DVAuraParameter(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        reader.ReadArray<float>(32, Values);
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
        writer.WriteArrayFixedLength(Values, 32);
    }
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