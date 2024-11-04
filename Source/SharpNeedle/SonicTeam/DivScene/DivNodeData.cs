namespace SharpNeedle.SonicTeam.DivScene;

public class DivDPath : IDivDataBlock
{
    public Matrix4x4 Transform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivDPath() { }
    public DivDPath(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Transform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Transform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DivDCamera : IDivDataBlock
{
    public int Field00 { get; set; }
    public int FrameCount { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public List<float> FrameTimes { get; set; } = new List<float>();
    public List<float> FrameData { get; set; } = new List<float>();

    public DivDCamera() { }
    public DivDCamera(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
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

    public void Write(BinaryObjectWriter writer, GameType game)
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

public class DivDCameraMotion : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DivDCameraMotion() { }
    public DivDCameraMotion(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
    }
}

public class DivDModel : IDivDataBlock
{
    public int Field00 { get; set; }
    public string ModelName { get; set; }
    public string SkeletonName { get; set; }
    public string Field84 { get; set; }

    public DivDModel() { }
    public DivDModel(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        ModelName = reader.ReadDivString(64);
        SkeletonName = reader.ReadDivString(64);
        Field84 = reader.ReadDivString(64);

        reader.Skip(76);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDivString(ModelName, 64);
        writer.WriteDivString(SkeletonName, 64);
        writer.WriteDivString(Field84, 64);

        writer.WriteNulls(76);
    }
}

public class DivDMotionModel : IDivDataBlock
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public string Field10 { get; set; }
    public float Field18 { get; set; }

    public DivDMotionModel() { }
    public DivDMotionModel(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.ReadDivString(8);
        Field18 = reader.Read<float>();

        reader.Skip(20);
    }

    public void Write(BinaryObjectWriter writer, GameType game)
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

public class DivDAttachment : IDivDataBlock
{
    public int Field00 { get; set; }
    public string NodeName { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DivDAttachment() { }
    public DivDAttachment(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        NodeName = reader.ReadDivString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDivString(NodeName, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DivDParameter : IDivDataBlock
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
    public IDivDataBlock Parameter { get; set; }
    
    public DivDParameter() { }
    public DivDParameter(BinaryObjectReader reader, GameType game, int size)
    {
        UnknownDataSize = size - 8;
        Read(reader, game);
    }

    public DivDParameter(ParameterType type, float startTime, float endTime)
    {
        Type = (int)type;
        StartTime = startTime;
        EndTime = endTime;
    }

    public DivDParameter(ParameterType type, float startTime, float endTime, IDivDataBlock parameter) : this(type, startTime, endTime)
    {
        Parameter = parameter;
    }

    public void ReadFrontiersParameter(BinaryObjectReader reader, int type)
    {
        switch ((FrontiersParams)type)
        {
            case FrontiersParams.DepthOfField:
                Parameter = new DivPDOF(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ColorCorrection:
                Parameter = new DivPColorCorrection(reader, GameType.Frontiers);
                break;

            case FrontiersParams.CameraExposure:
                Parameter = new DivPCameraExposure(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ShadowResolution:
                Parameter = new DivPShadowRes(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ChromaticAberration:
                Parameter = new DivPChromaAberr(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Vignette:
                Parameter = new DivPVignette(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Fade:
                Parameter = new DivPFade(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Letterbox:
                Parameter = new DivPLetterbox(reader, GameType.Frontiers);
                break;

            case FrontiersParams.BossCutoff:
                Parameter = new DivPBossCutoff(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Subtitle:
                Parameter = new DivPSubtitle(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Sound:
                Parameter = new DivPSound(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Time:
                Parameter = new DivPTime(reader, GameType.Frontiers);
                break;

            case FrontiersParams.CameraBlur:
                Parameter = new DivPCameraBlur(reader, GameType.Frontiers);
                break;

            case FrontiersParams.GeneralPurposeTrigger:
                Parameter = new DivPGeneralTrigger(reader, GameType.Frontiers);
                break;

            case FrontiersParams.DitherDepth:
                Parameter = new DivPDitherDepth(reader, GameType.Frontiers);
                break;

            case FrontiersParams.QTE:
                Parameter = new DivPQTE(reader, GameType.Frontiers);
                break;

            case FrontiersParams.ASMForcedOverwrite:
                Parameter = new DivPAnimStateMachine(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Aura:
                Parameter = new DivPAura(reader, GameType.Frontiers);
                break;

            case FrontiersParams.TimescaleChange:
                Parameter = new DivPTimescale(reader, GameType.Frontiers);
                break;

            case FrontiersParams.CyberNoise:
                Parameter = new DivPCyberNoise(reader, GameType.Frontiers);
                break;

            case FrontiersParams.MovieDisplay:
                Parameter = new DivPMovieDisplay(reader, GameType.Frontiers);
                break;

            case FrontiersParams.Weather:
                Parameter = new DivPWeather(reader, GameType.Frontiers);
                break;

            case FrontiersParams.TheEndCable:
                Parameter = new DivPTheEndCable(reader, GameType.Frontiers);
                break;

            case FrontiersParams.FinalBossLighting:
                Parameter = new DivPFinalBossLighting(reader, GameType.Frontiers);
                break;

            default:
                Parameter = new DivPUnknown(reader, GameType.Frontiers, UnknownDataSize);
                break;
        }
    }

    public void ReadShadowGensParameter(BinaryObjectReader reader, int type)
    {
        switch ((ShadowGensParams)type)
        {
            case ShadowGensParams.Subtitle:
                Parameter = new DivPSubtitle(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.Sound:
                Parameter = new DivPSound(reader, GameType.ShadowGenerations);
                break;

            case ShadowGensParams.TimeStop:
                Parameter = new DivPTimeStop(reader, GameType.ShadowGenerations);
                break;

            default:
                Parameter = new DivPUnknown(reader, GameType.ShadowGenerations, UnknownDataSize);
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
                Parameter = new DivPUnknown(reader, game, UnknownDataSize);
                break;
        }
    }

    public void Read(BinaryObjectReader reader, GameType game)
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
                    Parameter = new DivPDrawingOff(reader, GameType.Common);
                    break;

                case ParameterType.PathAdjust:
                    Parameter = new DivPPathAdjust(reader, GameType.Common);
                    break;

                case ParameterType.Effect:
                    Parameter = new DivPEffect(reader, GameType.Common);
                    break;

                case ParameterType.CullDisabled:
                    Parameter = new DivPCullDisabled(reader, GameType.Common);
                    break;

                case ParameterType.UVAnimation:
                    Parameter = new DivPAnimUV(reader, GameType.Common);
                    break;

                case ParameterType.MaterialAnimation:
                    Parameter = new DivPAnimMaterial(reader, GameType.Common);
                    break;

                case ParameterType.CompositeAnimation:
                    Parameter = new DivPCompositeAnim(reader, GameType.Common);
                    break;

                case ParameterType.GameCamera:
                    Parameter = new DivPGameCamera(reader, GameType.Common);
                    break;

                case ParameterType.MaterialParameter:
                    Parameter = new DivPMaterialParam(reader, GameType.Common);
                    break;

                default:
                    Parameter = new DivPUnknown(reader, GameType.Common, UnknownDataSize);
                    break;
            }
        } 
        else
        {
            ReadGameSpecificParameter(reader, game, Type);
        }
    }

    public void Write(BinaryObjectWriter writer, GameType game)
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
            Parameter.Write(writer, game);
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