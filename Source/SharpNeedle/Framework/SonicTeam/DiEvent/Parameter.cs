namespace SharpNeedle.Framework.SonicTeam.DiEvent;

using SharpNeedle.Structs;

public abstract class BaseParam : IBinarySerializable<GameType>
{
    public virtual void Read(BinaryObjectReader reader, GameType game) { }

    public virtual void Write(BinaryObjectWriter reader, GameType game) { }

    public virtual int GetTypeID(GameType game) { return 0; }
}

public class UnknownParam : BaseParam
{
    public byte[] Data { get; set; } = [];
    public int Size { get; set; }
    public int Type { get; set; }

    public UnknownParam() { }

    public UnknownParam(BinaryObjectReader reader, int size, int type)
    {
        Size = size;
        Type = type;
        Data = new byte[Size * 4];

        Read(reader, GameType.Common);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        reader.ReadArray<byte>(Size * 4, Data);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteArrayFixedLength(Data, Size * 4);
    }

    public override int GetTypeID(GameType game) { return Type; }
}

// Common parameters

class DrawOffParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public DrawOffParam() { }
    public DrawOffParam(BinaryObjectReader reader, GameType game)
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

    public override int GetTypeID(GameType game) { return (int)ParameterType.DrawingOff; }
}

public class PathAdjustParam : BaseParam
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public PathAdjustParam() { }
    public PathAdjustParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.PathAdjust; }
}

public class CameraShakeParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public int Field10 { get; set; }
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }

    public CameraShakeParam() { }
    public CameraShakeParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<int>();
        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CameraShake; }
}

public class CameraShakeLoopParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float[] CurveData { get; set; } = new float[64];

    public CameraShakeLoopParam() { }
    public CameraShakeLoopParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        reader.ReadArray<float>(64, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.WriteArrayFixedLength(CurveData, 64);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CameraShakeLoop; }
}

public class EffectParam : BaseParam
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Field84 { get; set; }
    public int Field88 { get; set; }
    public int Field8C { get; set; }
    public int Field90 { get; set; }
    public int Field94 { get; set; }
    public int Field98 { get; set; }
    public int Field9C { get; set; }
    public int FieldA0 { get; set; }
    public float[] AnimationData { get; set; } = new float[128];

    public EffectParam() { }
    public EffectParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Name = reader.ReadDiString(64);
        Field84 = reader.Read<int>();
        Field88 = reader.Read<int>();
        Field8C = reader.Read<int>();
        Field90 = reader.Read<int>();
        Field94 = reader.Read<int>();
        Field98 = reader.Read<int>();
        Field9C = reader.Read<int>();
        FieldA0 = reader.Read<int>();
        reader.ReadArray<float>(128, AnimationData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.WriteDiString(Name);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
        writer.Write(Field90);
        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(FieldA0);
        writer.WriteArrayFixedLength(AnimationData, 128);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.Effect; }
}

public class CullDisabledParam : BaseParam
{
    public CullDisabledParam() { }
    public CullDisabledParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game) { }

    public override void Write(BinaryObjectWriter writer, GameType game) { }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CullDisabled; }
}

public class UVAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public UVAnimParam() { }

    public UVAnimParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDiString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.UVAnimation; }
}

public class VisibilityAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public VisibilityAnimParam() { }

    public VisibilityAnimParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDiString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.VisibilityAnimation; }
}

public class MaterialAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Field44 { get; set; }
    public float Field48 { get; set; }
    public int Field4C { get; set; }
    public int Field50 { get; set; }

    public MaterialAnimParam() { }

    public MaterialAnimParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Name = reader.ReadDiString(64);
        Field44 = reader.Read<int>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<int>();
        Field50 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(Name, 64);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
        writer.Write(Field50);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.MaterialAnimation; }
}

public class CompositeAnimParam : BaseParam
{
    public int Field00 { get; set; }
    public string StateName { get; set; } = string.Empty;
    public Animation[] Animations { get; set; } = new Animation[16];
    public int ActiveAnimCount { get; set; }

    public CompositeAnimParam() { }

    public CompositeAnimParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        StateName = reader.ReadString(StringBinaryFormat.FixedLength, 12);
        Animations = reader.ReadObjectArray<Animation>(16);
        ActiveAnimCount = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, StateName, 12);
        writer.WriteObjectCollection(Animations);
        writer.Write(ActiveAnimCount);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CompositeAnimation; }

    public class Animation : IBinarySerializable
    {
        public int Type { get; set; }
        public string Name { get; set; }

        public Animation()
        {
            Type = (int)AnimationType.None;
            Name = "";
        }

        public void Read(BinaryObjectReader reader)
        {
            Type = reader.Read<int>();
            Name = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Type);
            writer.WriteString(StringBinaryFormat.FixedLength, Name, 64);
        }
    }

    public enum AnimationType
    {
        None = 0,
        PXD = 1,
        UV = 2,
        Visibility = 3,
        Material = 4,
    }
}

public class SonicCameraParam : BaseParam
{
    public uint Flags { get; set; }
    public uint Field04 { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public Vector3 Field10 { get; set; }
    public uint Field1C { get; set; }
    public uint Field20 { get; set; }
    public uint Field24 { get; set; }
    public Vector3 Field28 { get; set; }
    public byte[] UnknownData { get; set; } = new byte[268];

    public SonicCameraParam() { }
    public SonicCameraParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Flags = reader.Read<uint>();
        Field04 = reader.Read<uint>();
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<Vector3>();
        Field1C = reader.Read<uint>();
        Field20 = reader.Read<uint>();
        Field24 = reader.Read<uint>();
        Field28 = reader.Read<Vector3>();
        reader.ReadArray<byte>(268, UnknownData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Flags);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.WriteArrayFixedLength(UnknownData, 268);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.SonicCamera; }
}

public class GameCameraParam : BaseParam
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

    public GameCameraParam() { }
    public GameCameraParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
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

    public override void Write(BinaryObjectWriter writer, GameType game)
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

    public override int GetTypeID(GameType game) { return (int)ParameterType.GameCamera; }
}

public class ControllerVibrationParam : BaseParam
{
    public int Field00 { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public uint Field84 { get; set; }
    public uint Field88 { get; set; }
    public uint Field8C { get; set; }

    public ControllerVibrationParam() { }

    public ControllerVibrationParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Group = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Mode = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field84 = reader.Read<uint>();
        Field88 = reader.Read<uint>();
        Field8C = reader.Read<uint>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, Group, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Mode, 64);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.ControllerVibration; }
}

public class MaterialParameterParam : BaseParam
{
    public string MaterialName { get; set; } = string.Empty;
    public string ParamName { get; set; } = string.Empty;
    public uint Type { get; set; }
    public uint[] UnknownData { get; set; } = new uint[40];

    public MaterialParameterParam() { }

    public MaterialParameterParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        MaterialName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        ParamName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Type = reader.Read<uint>();
        reader.ReadArray<uint>(40, UnknownData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, MaterialName, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, ParamName, 64);
        writer.Write(Type);
        writer.WriteArrayFixedLength(UnknownData, 40);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.MaterialParameter; }

    public enum ParamType
    {
        Float = 3
    }
}

// Game-specific

public class DOFParam : BaseParam
{
    public int Field00 { get; set; }
    public Endpoint EndpointA { get; set; } = new Endpoint();
    public Endpoint EndpointB { get; set; } = new Endpoint();
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public int Field38 { get; set; }
    public int Field3C { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public DOFParam() { }
    public DOFParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        EndpointA = reader.ReadObject<Endpoint>();
        EndpointB = reader.ReadObject<Endpoint>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<int>();
        Field3C = reader.Read<int>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteObject(EndpointA);
        writer.WriteObject(EndpointB);
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
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public class Endpoint : IBinarySerializable
    {
        public float Focus { get; set; }
        public float FocusRange { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        public Endpoint() { }

        public Endpoint(BinaryObjectReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryObjectReader reader)
        {
            Focus = reader.Read<float>();
            FocusRange = reader.Read<float>();
            Near = reader.Read<float>();
            Far = reader.Read<float>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Focus);
            writer.Write(FocusRange);
            writer.Write(Near);
            writer.Write(Far);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.DepthOfField;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.DepthOfField;

            default:
                return 0;
        }
    }
}

public class MovieDisplayParam : BaseParam
{
    public MovieDisplayParam() { }
    public MovieDisplayParam(BinaryObjectReader reader, GameType game)
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
                return (int)FrontiersParams.MovieDisplay;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.MovieDisplay;

            default:
                return 0;
        }
    }
}

public class FadeParam : BaseParam
{
    Color<uint> Color { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public FadeParam() { }
    public FadeParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Color = reader.Read<Color<uint>>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Color);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Fade;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Fade;

            default:
                return 0;
        }
    }
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

public class ChromaAberrationParam : BaseParam
{
    public Endpoint EndpointA { get; set; } = new Endpoint();
    public float Field20 { get; set; }
    public Endpoint EndpointB { get; set; } = new Endpoint();
    public float[] CurveData { get; set; } = new float[32];

    public ChromaAberrationParam() { }
    public ChromaAberrationParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        EndpointA = reader.ReadObject<Endpoint>();
        Field20 = reader.Read<float>();
        EndpointB = reader.ReadObject<Endpoint>();

        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteObject(EndpointA);
        writer.Write(Field20);
        writer.WriteObject(EndpointB);

        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ChromaticAberration;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.ChromaticAberration;

            default:
                return 0;
        }
    }

    public class Endpoint : IBinarySerializable
    {
        public Color<float> ColorOffset { get; set; }
        public float SphereCurve { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Position { get; set; }

        public Endpoint() { }

        public Endpoint(BinaryObjectReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryObjectReader reader)
        {
            ColorOffset = new Color<float>(reader.Read<float>(), reader.Read<float>(), reader.Read<float>(), 1.0f);
            SphereCurve = reader.Read<float>();
            Scale = reader.Read<Vector2>();
            Position = reader.Read<Vector2>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(new Vector3(ColorOffset.R, ColorOffset.G, ColorOffset.B));
            writer.Write(SphereCurve);
            writer.Write(Scale);
            writer.Write(Position);
        }
    }
}

public class SoundParam : BaseParam
{
    public string CueName { get; set; } = string.Empty;
    public int Field40 { get; set; }
    public int Field44 { get; set; }

    public SoundParam() { }

    public SoundParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        CueName = reader.ReadDiString(64);
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteDiString(CueName);
        writer.Write(Field40);
        writer.Write(Field44);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Sound;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Sound;

            default:
                return 0;
        }
    }
}

public class SubtitleParam : BaseParam
{
    public string CellName { get; set; } = string.Empty;
    public SubtitleLanguage Language { get; set; }
    public int Field14 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public string CellName2 { get; set; } = string.Empty;

    public SubtitleParam() { }
    public SubtitleParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        if(game == GameType.ShadowGenerations)
        {
            CellName = reader.ReadDiString(32);
            Language = (SubtitleLanguage)reader.Read<int>();
            Field14 = reader.Read<int>();
            Field24 = reader.Read<int>();
            Field28 = reader.Read<int>();
            CellName2 = reader.ReadDiString(32);
        }
        else
        {
            CellName = reader.ReadDiString(16);
            Language = (SubtitleLanguage)reader.Read<int>();
            Field14 = reader.Read<int>();
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        if(game == GameType.ShadowGenerations)
        {
            writer.WriteDiString(CellName, 32);
            writer.Write((int)Language);
            writer.Write(Field14);
            writer.Write(Field24);
            writer.Write(Field28);
            writer.WriteDiString(CellName2, 32);
        }
        else
        {
            writer.WriteDiString(CellName, 16);
            writer.Write((int)Language);
            writer.Write(Field14);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Subtitle;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Subtitle;

            default:
                return 0;
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
        Chinese = 9,
        ChineseSimplified = 10,
        Korean = 11
    }
}

public class QTEParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
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
    public int[] Field80 { get; set; } = new int[32];
    public string Field100 { get; set; } = string.Empty;
    public string Field140 { get; set; } = string.Empty;
    public string Field180 { get; set; } = string.Empty;
    public uint Field1C0 { get; set; }

    public QTEParam() { }
    public QTEParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
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
        reader.ReadArray<int>(32, Field80);

        Field100 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field140 = reader.ReadString(StringBinaryFormat.FixedLength, 64);

        if(game == GameType.ShadowGenerations)
        {
            Field180 = reader.ReadString(StringBinaryFormat.FixedLength, 64);
            Field1C0 = reader.Read<uint>();
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
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
        writer.WriteArrayFixedLength(Field80, 32);

        writer.WriteString(StringBinaryFormat.FixedLength, Field100, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Field140, 64);

        if(game == GameType.ShadowGenerations)
        {
            writer.WriteString(StringBinaryFormat.FixedLength, Field180, 64);
            writer.Write(Field1C0);
        }
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.QTE;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.QTE;

            default:
                return 0;
        }
    }
}

public class TimescaleParam : BaseParam
{
    public int Field00 { get; set; }
    public float Scale { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }

    public TimescaleParam() { }
    public TimescaleParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Scale = reader.Read<float>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Scale);
        writer.Write(Field08);
        writer.Write(Field0C);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.TimescaleChange;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimescaleChange;

            default:
                return 0;
        }
    }
}

public class VignetteParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public float Field28 { get; set; }
    public float Field2C { get; set; }
    public float Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float Field44 { get; set; }
    public float Field48 { get; set; }
    public float Field4C { get; set; }
    public float Field50 { get; set; }
    public float Field54 { get; set; }
    public float Field58 { get; set; }
    public float Field5C { get; set; }
    public float Field60 { get; set; }
    public float Field64 { get; set; }
    public float Field68 { get; set; }
    public float Field6C { get; set; }
    public float Field70 { get; set; }
    public float Field74 { get; set; }
    public float Field78 { get; set; }
    public float Field7C { get; set; }
    public float Field80 { get; set; }
    public float Field84 { get; set; }
    public int Field88 { get; set; }
    public float Field8C { get; set; }
    public float Field90 { get; set; }
    public float Field94 { get; set; }
    public float Field98 { get; set; }
    public float Field9C { get; set; }
    public float FieldA0 { get; set; }
    public float FieldA4 { get; set; }
    public float FieldA8 { get; set; }
    public float FieldAC { get; set; }
    public float FieldB0 { get; set; }
    public float FieldB4 { get; set; }
    public float FieldB8 { get; set; }
    public float FieldBC { get; set; }
    public float FieldC0 { get; set; }
    public float FieldC4 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public VignetteParam() { }
    public VignetteParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<float>();
        Field30 = reader.Read<float>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<float>();
        Field3C = reader.Read<float>();
        Field40 = reader.Read<float>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<float>();
        Field4C = reader.Read<float>();
        Field50 = reader.Read<float>();
        Field54 = reader.Read<float>();
        Field58 = reader.Read<float>();
        Field5C = reader.Read<float>();
        Field60 = reader.Read<float>();
        Field64 = reader.Read<float>();
        Field68 = reader.Read<float>();
        Field6C = reader.Read<float>();
        Field70 = reader.Read<float>();
        Field74 = reader.Read<float>();
        Field78 = reader.Read<float>();
        Field7C = reader.Read<float>();
        Field80 = reader.Read<float>();
        Field84 = reader.Read<float>();
        Field88 = reader.Read<int>();
        Field8C = reader.Read<float>();
        Field90 = reader.Read<float>();
        Field94 = reader.Read<float>();
        Field98 = reader.Read<float>();
        Field9C = reader.Read<float>();
        FieldA0 = reader.Read<float>();
        FieldA4 = reader.Read<float>();
        FieldA8 = reader.Read<float>();
        FieldAC = reader.Read<float>();
        FieldB0 = reader.Read<float>();
        FieldB4 = reader.Read<float>();
        FieldB8 = reader.Read<float>();
        FieldBC = reader.Read<float>();
        FieldC0 = reader.Read<float>();
        FieldC4 = reader.Read<float>();

        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
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
        writer.Write(Field80);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
        writer.Write(Field90);
        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(FieldA0);
        writer.Write(FieldA4);
        writer.Write(FieldA8);
        writer.Write(FieldAC);
        writer.Write(FieldB0);
        writer.Write(FieldB4);
        writer.Write(FieldB8);
        writer.Write(FieldBC);
        writer.Write(FieldC0);
        writer.Write(FieldC4);

        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Vignette;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.Vignette;

            default:
                return 0;
        }
    }
}

public class BossNameParam : BaseParam
{
    public int Field00 { get; set; }
    public int NameType { get; set; }

    public BossNameParam() { }
    public BossNameParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        NameType = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(NameType);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.BossName;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.BossName;

            default:
                return 0;
        }
    }

    public enum FrontiersNames
    {
        Giant = 0,
        Dragon,
        Knight,
        Rifle,
        TheEnd,
        RifleBeast
    }

    public enum ShadowGensNames
    {
        Biolizard = 0,
        MetalOverlord,
        Mephiles,
        DevilDoom,
        PerfectBlackDoom
    }
}

public class AuraParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public float Field10 { get; set; }
    public float Field14 { get; set; }
    public float Field18 { get; set; }
    public float Field1C { get; set; }
    public float Field20 { get; set; }
    public int Field24 { get; set; }
    public int Field28 { get; set; }
    public int Field2C { get; set; }
    public int Field30 { get; set; }
    public float Field34 { get; set; }
    public float Field38 { get; set; }
    public float Field3C { get; set; }
    public float Field40 { get; set; }
    public float Field44 { get; set; }
    public int Field48 { get; set; }
    public float[] ValuesTimeline { get; set; } = new float[32];

    public AuraParam() { }
    public AuraParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<float>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<float>();
        Field20 = reader.Read<float>();
        Field24 = reader.Read<int>();
        Field28 = reader.Read<int>();
        Field2C = reader.Read<int>();
        Field30 = reader.Read<int>();
        Field34 = reader.Read<float>();
        Field38 = reader.Read<float>();
        Field3C = reader.Read<float>();
        Field40 = reader.Read<float>();
        Field44 = reader.Read<float>();
        Field48 = reader.Read<int>();
        reader.ReadArray<float>(32, ValuesTimeline);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
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
        writer.WriteArrayFixedLength(ValuesTimeline, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Aura;

            case GameType.ShadowGenerations:
                return 0;

            default:
                return 0;
        }
    }
}

public class TheEndCableParam : BaseParam
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public float[] Field08 { get; set; } = new float[1024];

    public TheEndCableParam() { }
    public TheEndCableParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        reader.ReadArray<float>(1024, Field08);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.WriteArrayFixedLength(Field08, 1024);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.TheEndCable;

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

public class CameraBlurParam : BaseParam
{
    public uint Field00 { get; set; }
    public uint Field04 { get; set; }
    public float Field08 { get; set; }
    public float[] CurveData { get; set; } = new float[32];
    public uint Flags { get; set; }

    public CameraBlurParam() { }
    public CameraBlurParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<uint>();
        Field08 = reader.Read<float>();
        reader.ReadArray<float>(32, CurveData);
        Flags = reader.Read<uint>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.WriteArrayFixedLength(CurveData, 32);
        writer.Write(Flags);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.CameraBlur;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.CameraBlur;

            default:
                return 0;
        }
    }
}

public class ShadowResolutionParam : BaseParam
{
    public Vector2Int Resolution { get; set; }

    public ShadowResolutionParam() { }
    public ShadowResolutionParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Resolution = reader.Read<Vector2Int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Resolution);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ShadowResolution;

            default:
                return 0;
        }
    }
}

public class TimeParam : BaseParam
{
    public uint Field00 { get; set; }
    public uint Field04 { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }
    public uint Field14 { get; set; }
    public uint Field18 { get; set; }
    public uint Field1C { get; set; }
    public uint Field20 { get; set; }
    public float[] DataCurve { get; set; } = new float[32];

    public TimeParam() { }
    public TimeParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<uint>();
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<uint>();
        Field1C = reader.Read<uint>();
        Field20 = reader.Read<uint>();
        reader.ReadArray<float>(32, DataCurve);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
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
        writer.WriteArrayFixedLength(DataCurve, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Time;

            default:
                return 0;
        }
    }
}

public class WeatherParam : BaseParam
{
    public uint Field00 { get; set; }
    public float[] DataCurve { get; set; } = new float[32];

    public WeatherParam() { }
    public WeatherParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        reader.ReadArray<float>(32, DataCurve);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteArrayFixedLength(DataCurve, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.Weather;

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

public class CyberNoiseParam : BaseParam
{
    public uint Field00 { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public CyberNoiseParam() { }
    public CyberNoiseParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.CyberNoise;

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

public class CameraExposureParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }
    public uint Field14 { get; set; }
    public uint Field18 { get; set; }
    public uint Field1C { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public CameraExposureParam() { }
    public CameraExposureParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<uint>();
        Field1C = reader.Read<uint>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.CameraExposure;

            default:
                return 0;
        }
    }
}

public class ColorCorrectionParam : BaseParam
{
    public uint Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }
    public float Field0C { get; set; }
    public float Field10 { get; set; }
    public uint Field14 { get; set; }
    public float Field18 { get; set; }
    public uint Field1C { get; set; }
    public float[] CurveData { get; set; } = new float[32];

    public ColorCorrectionParam() { }
    public ColorCorrectionParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<uint>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
        Field0C = reader.Read<float>();
        Field10 = reader.Read<float>();
        Field14 = reader.Read<uint>();
        Field18 = reader.Read<float>();
        Field1C = reader.Read<uint>();
        reader.ReadArray<float>(32, CurveData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.WriteArrayFixedLength(CurveData, 32);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ColorCorrection;

            default:
                return 0;
        }
    }
}

public class TimeStopParam : BaseParam
{
    public int Field00 { get; set; }
    public float Field04 { get; set; }
    public float Field08 { get; set; }

    public TimeStopParam() { }
    public TimeStopParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Field04 = reader.Read<float>();
        Field08 = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(Field08);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStop;

            default:
                return 0;
        }
    }

    public enum ParamType
    {
        Float = 3
    }
}

public class TimeStopControlParam : BaseParam
{
    public int Behavior { get; set; }
    public float Field04 { get; set; }
    public float TransitionDuration { get; set; }

    public TimeStopControlParam() { }
    public TimeStopControlParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Behavior = reader.Read<int>();
        Field04 = reader.Read<float>();
        TransitionDuration = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Behavior);
        writer.Write(Field04);
        writer.Write(TransitionDuration);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStopControl;

            default:
                return 0;
        }
    }

    public enum BehaviorMode
    {
        End = 1,
        Begin = 2,
    }
}

public class TimeStopObjectBehaviorParam : BaseParam
{
    public int Mode { get; set; }

    public TimeStopObjectBehaviorParam() { }
    public TimeStopObjectBehaviorParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Mode = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Mode);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.TimeStopObjectBehavior;

            default:
                return 0;
        }
    }
}

public class FalloffToggleParam : BaseParam
{
    public float Intensity { get; set; }

    public FalloffToggleParam() { }
    public FalloffToggleParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Intensity = reader.Read<float>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Intensity);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.FalloffToggle;

            default:
                return 0;
        }
    }
}

public class ShadowAfterimageParam : BaseParam
{
    public Color<int> Color { get; set; }
    public int Field10 { get; set; } // Possibly afterimage count?
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public int Field2C { get; set; }

    public ShadowAfterimageParam() { }
    public ShadowAfterimageParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Color = reader.Read<Color<int>>();
        Field10 = reader.Read<int>();
        Field14 = reader.Read<int>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();
        Field20 = reader.Read<int>();
        Field24 = reader.Read<float>();
        Field28 = reader.Read<float>();
        Field2C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Color);
        writer.Write(Field10);
        writer.Write(Field14);
        writer.Write(Field18);
        writer.Write(Field1C);
        writer.Write(Field20);
        writer.Write(Field24);
        writer.Write(Field28);
        writer.Write(Field2C);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.ShadowAfterimage;

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