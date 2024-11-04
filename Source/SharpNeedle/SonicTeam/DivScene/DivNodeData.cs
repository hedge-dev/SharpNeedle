namespace SharpNeedle.SonicTeam.DivScene;

public abstract class DivNodeData : IBinarySerializable
{
    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}

public class DivDPath : DivNodeData
{
    public Matrix4x4 Transform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivDPath() { }
    public DivDPath(BinaryObjectReader reader)
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

public class DivDCamera : DivNodeData
{
    public int Field00 { get; set; }
    public int FrameCount { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public List<float> FrameTimes { get; set; } = new List<float>();
    public List<float> FrameData { get; set; } = new List<float>();

    public DivDCamera() { }
    public DivDCamera(BinaryObjectReader reader)
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

public class DivDCameraMotion : DivNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DivDCameraMotion() { }
    public DivDCameraMotion(BinaryObjectReader reader)
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

public class DivDModel : DivNodeData
{
    public int Field00 { get; set; }
    public string ModelName { get; set; }
    public string SkeletonName { get; set; }
    public string Field84 { get; set; }

    public DivDModel() { }
    public DivDModel(BinaryObjectReader reader)
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

public class DivDMotionModel : DivNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public string Field10 { get; set; }
    public float Field18 { get; set; }

    public DivDMotionModel() { }
    public DivDMotionModel(BinaryObjectReader reader)
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

public class DivDAttachment : DivNodeData
{
    public int Field00 { get; set; }
    public string NodeName { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivDAttachment() { }
    public DivDAttachment(BinaryObjectReader reader)
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

public class DivDParameter : DivNodeData
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
    
    public DivDParameter() { }
    public DivDParameter(BinaryObjectReader reader, int size)
    {
        UnknownDataSize = size - 8;
        Read(reader);
    }

    public DivDParameter(ParameterType type, float startTime, float endTime)
    {
        Type = (int)type;
        StartTime = startTime;
        EndTime = endTime;
    }

    public DivDParameter(ParameterType type, float startTime, float endTime, DivParameter parameter) : this(type, startTime, endTime)
    {
        Parameter = parameter;
    }

    public void ReadFrontiersParameter(BinaryObjectReader reader, int type)
    {
        switch ((FrontiersParams)type)
        {
            case FrontiersParams.DepthOfField:
                Parameter = new DivPDOF(reader);
                break;

            case FrontiersParams.ColorCorrection:
                Parameter = new DivPColorCorrection(reader);
                break;

            case FrontiersParams.CameraExposure:
                Parameter = new DivPCameraExposure(reader);
                break;

            case FrontiersParams.ShadowResolution:
                Parameter = new DivPShadowRes(reader);
                break;

            case FrontiersParams.ChromaticAberration:
                Parameter = new DivPChromaAberr(reader);
                break;

            case FrontiersParams.Vignette:
                Parameter = new DivPVignette(reader);
                break;

            case FrontiersParams.Fade:
                Parameter = new DivPFade(reader);
                break;

            case FrontiersParams.Letterbox:
                Parameter = new DivPLetterbox(reader);
                break;

            case FrontiersParams.BossCutoff:
                Parameter = new DivPBossCutoff(reader);
                break;

            case FrontiersParams.Subtitle:
                Parameter = new DivPSubtitle(reader);
                break;

            case FrontiersParams.Sound:
                Parameter = new DivPSound(reader);
                break;

            case FrontiersParams.Time:
                Parameter = new DivPTime(reader);
                break;

            case FrontiersParams.CameraBlur:
                Parameter = new DivPCameraBlur(reader);
                break;

            case FrontiersParams.GeneralPurposeTrigger:
                Parameter = new DivPGeneralTrigger(reader);
                break;

            case FrontiersParams.DitherDepth:
                Parameter = new DivPDitherDepth(reader);
                break;

            case FrontiersParams.QTE:
                Parameter = new DivPQTE(reader);
                break;

            case FrontiersParams.ASMForcedOverwrite:
                Parameter = new DivPAnimStateMachine(reader);
                break;

            case FrontiersParams.Aura:
                Parameter = new DivPAura(reader);
                break;

            case FrontiersParams.TimescaleChange:
                Parameter = new DivPTimescale(reader);
                break;

            case FrontiersParams.CyberNoise:
                Parameter = new DivPCyberNoise(reader);
                break;

            case FrontiersParams.MovieDisplay:
                Parameter = new DivPMovieDisplay(reader);
                break;

            case FrontiersParams.Weather:
                Parameter = new DivPWeather(reader);
                break;

            case FrontiersParams.TheEndCable:
                Parameter = new DivPTheEndCable(reader);
                break;

            case FrontiersParams.FinalBossLighting:
                Parameter = new DivPFinalBossLighting(reader);
                break;

            default:
                Parameter = new DivPUnknown(reader, UnknownDataSize);
                break;
        }
    }

    public void ReadShadowGensParameter(BinaryObjectReader reader, int type)
    {
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
                Parameter = new DivPUnknown(reader, UnknownDataSize);
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
                    Parameter = new DivPDrawingOff(reader);
                    break;

                case ParameterType.PathAdjust:
                    Parameter = new DivPPathAdjust(reader);
                    break;

                case ParameterType.Effect:
                    Parameter = new DivPEffect(reader);
                    break;

                case ParameterType.CullDisabled:
                    Parameter = new DivPCullDisabled(reader);
                    break;

                case ParameterType.UVAnimation:
                    Parameter = new DivPAnimUV(reader);
                    break;

                case ParameterType.MaterialAnimation:
                    Parameter = new DivPAnimMaterial(reader);
                    break;

                case ParameterType.CompositeAnimation:
                    Parameter = new DivPCompositeAnim(reader);
                    break;

                case ParameterType.GameCamera:
                    Parameter = new DivPGameCamera(reader);
                    break;

                default:
                    Parameter = new DivPUnknown(reader, UnknownDataSize);
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