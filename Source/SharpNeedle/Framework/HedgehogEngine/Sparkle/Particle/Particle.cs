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
        ParticleName = reader.ReadStringPaddedByte(4);

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

        Color = reader.ReadObject<Vector4Int>();

        Gravity = reader.ReadVector4();
        ExternalForce = reader.ReadVector4();

        InitialDirection = reader.ReadVector4();
        InitialDirectionBias = reader.ReadVector4();

        InitialScale = reader.ReadVector4();
        InitialScaleBias = reader.ReadVector4();

        MeshName = reader.ReadStringPaddedByte(4);

        RotationXYZ = reader.ReadVector4();
        RotationXYZBias = reader.ReadVector4();
        InitialRotationXYZ = reader.ReadVector4();
        InitialRotationXYZBias = reader.ReadVector4();

        UVScrollParam = reader.ReadVector4();
        UVScrollParamAlpha = reader.ReadVector4();

        RefEffectName = reader.ReadStringPaddedByte(4);
        RefEffectEmitTimingType = reader.ReadInt32();
        RefEffectDelayTime = reader.ReadSingle();

        DirectionalVelocityRatio = reader.ReadSingle();
        DeflectionScale = reader.ReadSingle();
        SoftScale = reader.ReadSingle();
        VelocityOffset = reader.ReadSingle();
        UserData = reader.ReadSingle();

        MaterialName = reader.ReadStringPaddedByte(4);

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
        writer.WriteStringPaddedByte("ParticleChunk", 4);
        writer.WriteStringPaddedByte(ParticleName, 4);
        writer.WriteSingle(LifeTime);
        writer.WriteSingle(LifeTimeBias);

        writer.WriteSingle(RotationZ);
        writer.WriteSingle(RotationZBias);
        writer.WriteSingle(InitialRotationZ);
        writer.WriteSingle(InitialRotationZBias);

        writer.WriteSingle(InitialSpeed);
        writer.WriteSingle(InitialSpeedBias);

        writer.WriteSingle(ZOffset);
        writer.WriteSingle(LocusDiff);
        writer.WriteInt32(NumDivision);
        writer.WriteInt32((int)LocusUVType);
        writer.WriteInt32(IsBillboard ? 1 : 0);
        writer.WriteInt32(IsEmitterLocal ? 1 : 0);

        writer.WriteInt32((int)LayerType);
        writer.WriteInt32((int)PivotType);
        writer.WriteInt32((int)UVDescType);

        writer.WriteInt32((int)TextureIndexType);
        writer.WriteInt32(TextureIndexChangeInterval);
        writer.WriteInt32(TextureIndexChangeIntervalBias);
        writer.WriteInt32(InitialTextureIndex);

        writer.WriteInt32((int)DirectionType);
        writer.WriteInt32(ParticleDataFlags);

        writer.WriteObject(Color);

        writer.WriteVector4(Gravity);
        writer.WriteVector4(ExternalForce);
        writer.WriteVector4(InitialDirection);
        writer.WriteVector4(InitialDirectionBias);
        writer.WriteVector4(InitialScale);
        writer.WriteVector4(InitialScaleBias);

        writer.WriteStringPaddedByte(MeshName, 4);

        writer.WriteVector4(RotationXYZ);
        writer.WriteVector4(RotationXYZBias);
        writer.WriteVector4(InitialRotationXYZ);
        writer.WriteVector4(InitialRotationXYZBias);
        writer.WriteVector4(UVScrollParam);
        writer.WriteVector4(UVScrollParamAlpha);

        writer.WriteStringPaddedByte(RefEffectName, 4);
        writer.WriteInt32(RefEffectEmitTimingType);
        writer.WriteSingle(RefEffectDelayTime);

        writer.WriteSingle(DirectionalVelocityRatio);
        writer.WriteSingle(DeflectionScale);
        writer.WriteSingle(SoftScale);
        writer.WriteSingle(VelocityOffset);
        writer.WriteSingle(UserData);
        writer.WriteStringPaddedByte(MaterialName, 4);

        writer.WriteInt32(FieldU1);
        writer.WriteInt32(FieldU2);
        writer.WriteInt32(FieldU3);
        writer.WriteInt32(FieldU4);

        writer.WriteInt32(AnimCount);
        if (AnimCount > 0)
        {
            writer.WriteObject(ParticleAnim);
        }
    }
}