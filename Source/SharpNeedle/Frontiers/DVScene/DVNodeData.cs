namespace SharpNeedle.Frontiers.DVScene;

public abstract class DVNodeData : IBinarySerializable
{
    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}

public class DVTransformData : DVNodeData
{
    public Matrix4x4 Transform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DVTransformData() { }
    public DVTransformData(BinaryObjectReader reader)
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

public class DVCameraData : DVNodeData
{
    public int Field00 { get; set; }
    public int FrameCount { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public List<float> FrameTimes { get; set; } = new List<float>();
    public List<float> FrameData { get; set; } = new List<float>();

    public DVCameraData() { }
    public DVCameraData(BinaryObjectReader reader)
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

public class DVCameraMotionData : DVNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DVCameraMotionData() { }
    public DVCameraMotionData(BinaryObjectReader reader)
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

public class DVModelData : DVNodeData
{
    public int Field00 { get; set; }
    public string ModelName { get; set; }
    public string SkeletonName { get; set; }
    public string Field84 { get; set; }

    public DVModelData() { }
    public DVModelData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        ModelName = reader.ReadDVString(64);
        SkeletonName = reader.ReadDVString(64);
        Field84 = reader.ReadDVString(64);

        reader.Skip(76);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteDVString(ModelName, 64);
        writer.WriteDVString(SkeletonName, 64);
        writer.WriteDVString(Field84, 64);

        writer.WriteNulls(76);
    }
}

public class DVMotionData : DVNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public string Field10 { get; set; }
    public float Field18 { get; set; }

    public DVMotionData() { }
    public DVMotionData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.ReadDVString(8);
        Field18 = reader.Read<float>();

        reader.Skip(20);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.WriteDVString(Field10, 8);
        writer.Write(Field18);

        writer.WriteNulls(20);
    }
}

public class DVNodeAttachData : DVNodeData
{
    public int Field00 { get; set; }
    public string NodeName { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public DVNodeAttachData() { }
    public DVNodeAttachData(BinaryObjectReader reader)
        => Read(reader);

    public override void Read(BinaryObjectReader reader)
    {
        Field00 = reader.Read<int>();
        NodeName = reader.ReadDVString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Field00);
        writer.WriteDVString(NodeName, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }
}

public class DVAttributeData : DVNodeData
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
    public DVAttribute Attribute { get; set; }
    
    public DVAttributeData() { }
    public DVAttributeData(BinaryObjectReader reader, int size)
    {
        UnknownDataSize = size - 8;
        Read(reader);
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

        switch ((AttributeType)Type)
        {
            case AttributeType.DrawingOff:
                Attribute = new DVDrawingOffAttribute(reader);
                break;

            case AttributeType.PathAdjust:
                Attribute = new DVPathAdjustAttribute(reader);
                break;

            case AttributeType.Effect:
                Attribute = new DVEffectAttribute(reader);
                break;

            case AttributeType.CullDisabled:
                Attribute = new DVCullDisabledAttribute(reader);
                break;

            case AttributeType.UVAnimation:
                Attribute = new DVUVAnimAttribute(reader);
                break;

            case AttributeType.MaterialAnimation:
                Attribute = new DVMaterialAnimAttribute(reader);
                break;

            case AttributeType.GameCamera:
                Attribute = new DVGameCameraAttribute(reader);
                break;

            case AttributeType.ChromaticAberration:
                Attribute = new DVChromaAberrAttribute(reader);
                break;

            case AttributeType.Fade:
                Attribute = new DVFadeAttribute(reader);
                break;

            case AttributeType.Letterbox:
                Attribute = new DVLetterboxAttribute(reader);
                break;

            case AttributeType.BossCutoff:
                Attribute = new DVBossCutoffAttribute(reader);
                break;

            case AttributeType.Subtitle:
                Attribute = new DVSubtitleAttribute(reader);
                break;

            case AttributeType.Sound:
                Attribute = new DVSoundAttribute(reader);
                break;

            case AttributeType.QTE:
                Attribute = new DVQTEAttribute(reader);
                break;

            case AttributeType.Aura:
                Attribute = new DVAuraAttribute(reader);
                break;

            case AttributeType.TimescaleChange:
                Attribute = new DVTimescaleAttribute(reader);
                break;

            case AttributeType.MovieDisplay:
                Attribute = new DVMovieDisplayAttribute(reader);
                break;

            default:
                Attribute = new DVUnknownAttribute(reader, UnknownDataSize);
                break;
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
        
        if(Attribute != null)
            Attribute.Write(writer);
    }
}

public enum NodeType
{
    Transform = 1,
    Camera = 3,
    CameraMotion = 4,
    Model = 8,
    Motion = 10,
    NodeAttachment = 11,
    Attribute = 12
}