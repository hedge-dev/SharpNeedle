namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

using SharpNeedle.Structs;
public class Particle : IBinarySerializable
{
    public string? ParticleName { get; set; }
    public float LifeTime { get; set; }
    public float LifeTimeBias { get; set; }

    public float RotationZ { get; set; }
    public float RotationZBias { get; set; }
    public float InitialRotationZ { get; set; }
    public float InitialRotationZBias { get; set; }

    public float InitialSpeed { get; set; }
    public float InitialSpeedBias { get; set; }
    public float ZOffset { get; set; }
    public float LocusDiff { get; set; }

    public int NumDivision { get; set; }
    public LocusUVType LocusUVType { get; set; }

    public bool IsBillboard { get; set; }
    public bool IsEmitterLocal { get; set; }

    public LayerType LayerType { get; set; }
    public PivotType PivotType { get; set; }
    public UVDescType UVDescType { get; set; }
    public TextureIndexType TextureIndexType { get; set; }
    public int TextureIndexChangeInterval { get; set; }
    public int TextureIndexChangeIntervalBias { get; set; }
    public int InitialTextureIndex { get; set; }
    public DirectionType DirectionType { get; set; }
    public int ParticleDataFlags { get; set; }

    //ARGB
    public Vector4Int Color { get; set; }

    public Vector4 Gravity { get; set; }
    public Vector4 ExternalForce { get; set; }
    public Vector4 InitialDirection { get; set; }
    public Vector4 InitialDirectionBias { get; set; }
    public Vector4 InitialScale { get; set; }
    public Vector4 InitialScaleBias { get; set; }

    public string? MeshName { get; set; }

    public Vector4 RotationXYZ { get; set; }
    public Vector4 RotationXYZBias { get; set; }
    public Vector4 InitialRotationXYZ { get; set; }
    public Vector4 InitialRotationXYZBias { get; set; }
    public Vector4 UVScrollParam { get; set; }
    public Vector4 UVScrollParamAlpha { get; set; }

    public string? RefEffectName { get; set; }
    public int RefEffectEmitTimingType { get; set; }
    public float RefEffectDelayTime { get; set; }

    public float DirectionalVelocityRatio { get; set; }
    public float DeflectionScale { get; set; }
    public float SoftScale { get; set; }
    public float VelocityOffset { get; set; }
    public float UserData { get; set; }

    public string? MaterialName { get; set; }

    public int FieldU1 { get; set; }
    public int FieldU2 { get; set; }
    public int FieldU3 { get; set; }
    public int FieldU4 { get; set; }

    public int AnimCount { get; set; }
    public Animation? ParticleAnim { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        ParticleName = reader.ReadStringPaddedByte();

        LifeTime = reader.ReadSingle();
        LifeTimeBias = reader.ReadSingle();

        RotationZ = reader.ReadSingle();
        RotationZBias = reader.ReadSingle();
        InitialRotationZ = reader.ReadSingle();
        InitialRotationZBias = reader.ReadSingle();

        InitialSpeed = reader.ReadSingle();
        InitialSpeedBias = reader.ReadSingle();

        ZOffset = reader.ReadSingle();

        LocusDiff = reader.ReadSingle();
        NumDivision = reader.ReadInt32();
        LocusUVType = (LocusUVType)reader.ReadInt32();

        IsBillboard = reader.ReadUInt32() == 1;
        IsEmitterLocal = reader.ReadUInt32() == 1;

        LayerType = (LayerType)reader.ReadInt32();
        PivotType = (PivotType)reader.ReadInt32();
        UVDescType = (UVDescType)reader.ReadInt32();
        TextureIndexType = (TextureIndexType)reader.ReadInt32();
        TextureIndexChangeInterval = reader.ReadInt32();
        TextureIndexChangeIntervalBias = reader.ReadInt32();
        InitialTextureIndex = reader.ReadInt32();
        DirectionType = (DirectionType)reader.ReadInt32();
        ParticleDataFlags = reader.ReadInt32();

        Color = reader.Read<Vector4Int>();

        Gravity = reader.Read<Vector4>();
        ExternalForce = reader.Read<Vector4>();

        InitialDirection = reader.Read<Vector4>();
        InitialDirectionBias = reader.Read<Vector4>();

        InitialScale = reader.Read<Vector4>();
        InitialScaleBias = reader.Read<Vector4>();

        MeshName = reader.ReadStringPaddedByte();

        RotationXYZ = reader.Read<Vector4>();
        RotationXYZBias = reader.Read<Vector4>();
        InitialRotationXYZ = reader.Read<Vector4>();
        InitialRotationXYZBias = reader.Read<Vector4>();

        UVScrollParam = reader.Read<Vector4>();
        UVScrollParamAlpha = reader.Read<Vector4>();

        RefEffectName = reader.ReadStringPaddedByte();
        RefEffectEmitTimingType = reader.ReadInt32();
        RefEffectDelayTime = reader.ReadSingle();

        DirectionalVelocityRatio = reader.ReadSingle();
        DeflectionScale = reader.ReadSingle();
        SoftScale = reader.ReadSingle();
        VelocityOffset = reader.ReadSingle();
        UserData = reader.ReadInt32();

        MaterialName = reader.ReadStringPaddedByte();

        FieldU1 = reader.ReadInt32();
        FieldU2 = reader.ReadInt32();
        FieldU3 = reader.ReadInt32();
        FieldU4 = reader.ReadInt32();
        AnimCount = reader.ReadInt32();
        if (AnimCount > 0)
        {
            ParticleAnim = reader.ReadObject<Animation>();
        }
    }
    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte("ParticleChunk");
        writer.WriteStringPaddedByte(ParticleName);
        writer.Write(LifeTime);
        writer.Write(LifeTimeBias);

        writer.Write(RotationZ);
        writer.Write(RotationZBias);
        writer.Write(InitialRotationZ);
        writer.Write(InitialRotationZBias);

        writer.Write(InitialSpeed);
        writer.Write(InitialSpeedBias);

        writer.Write(ZOffset);
        writer.Write(LocusDiff);
        writer.Write(NumDivision);
        writer.Write(LocusUVType);
        writer.Write(IsBillboard ? 1 : 0);
        writer.Write(IsEmitterLocal ? 1 : 0);

        writer.Write(LayerType);
        writer.Write(PivotType);
        writer.Write(UVDescType);

        writer.Write(TextureIndexType);
        writer.Write(TextureIndexChangeInterval);
        writer.Write(TextureIndexChangeIntervalBias);
        writer.Write(InitialTextureIndex);

        writer.Write(DirectionType);
        writer.Write(ParticleDataFlags);

        writer.Write(Color);

        writer.Write(Gravity);
        writer.Write(ExternalForce);
        writer.Write(InitialDirection);
        writer.Write(InitialDirectionBias);
        writer.Write(InitialScale);
        writer.Write(InitialScaleBias);

        writer.WriteStringPaddedByte(MeshName);

        writer.Write(RotationXYZ);
        writer.Write(RotationXYZBias);
        writer.Write(InitialRotationXYZ);
        writer.Write(InitialRotationXYZBias);
        writer.Write(UVScrollParam);
        writer.Write(UVScrollParamAlpha);

        writer.WriteStringPaddedByte(RefEffectName);
        writer.Write(RefEffectEmitTimingType);
        writer.Write(RefEffectDelayTime);

        writer.Write(DirectionalVelocityRatio);
        writer.Write(DeflectionScale);
        writer.Write(SoftScale);
        writer.Write(VelocityOffset);
        writer.Write(UserData);
        writer.WriteStringPaddedByte(MaterialName);

        writer.Write(FieldU1);
        writer.Write(FieldU2);
        writer.Write(FieldU3);
        writer.Write(FieldU4);

        writer.Write(AnimCount);
        if (AnimCount > 0)
        {
            writer.WriteObject(ParticleAnim);
        }
    }
}