namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Emitter : IBinarySerializable
{
    public int ParticleCount { get; set; }
    public string? Name { get; set; }
    public int MaxGenerateCount { get; set; }
    public int GenerateCount { get; set; }
    public int ParticleDataFlags { get; set; }
    public bool Infinite { get; set; }
    public float InitialEmissionGap { get; set; }

    public Vector4 InitialPosition { get; set; }
    public Vector4 RotationXYZ { get; set; }
    public Vector4 RotationXYZBias { get; set; }
    public Vector4 InitialRotationXYZ { get; set; }
    public Vector4 InitialRotation { get; set; }

    public float InitialEmitterLifeTime { get; set; }
    public float EmitStartTime { get; set; }
    public int EmitCondition { get; set; }
    public int EmitterType { get; set; }

    public Cylinder? CylinderParams { get; set; }
    public Sphere? SphereParams { get; set; }
    public Vector4 Size { get; set; }
    public string? MeshName { get; set; }
    public int FieldU1 { get; set; }
    public int FieldU2 { get; set; }
    public int FieldU3 { get; set; }
    public int FieldU4 { get; set; }
    public int AnimCount { get; set; }
    public Animation? EmitterAnim { get; set; }
    public List<Particle> ParticleSaveLoad { get; set; } = [];
    
    public void Read(BinaryObjectReader reader)
    {
        // Emitter Params
        reader.ReadStringPaddedByte(4);
        ParticleCount = reader.ReadInt32();
        Name = reader.ReadStringPaddedByte(4);

        MaxGenerateCount = reader.ReadInt32();
        GenerateCount = reader.ReadInt32();
        ParticleDataFlags = reader.ReadInt32();
        Infinite = reader.ReadUInt32() == 1;
        InitialEmissionGap = reader.ReadSingle();

        InitialPosition = reader.ReadVector4();
        RotationXYZ = reader.ReadVector4();
        RotationXYZBias = reader.ReadVector4();
        InitialRotationXYZ = reader.ReadVector4();
        InitialRotation = reader.ReadVector4();

        InitialEmitterLifeTime = reader.ReadSingle();
        EmitStartTime = reader.ReadSingle();
        EmitCondition = reader.ReadInt32();
        EmitterType = reader.ReadInt32();

        CylinderParams = reader.ReadObject<Cylinder>();
        SphereParams = reader.ReadObject<Sphere>();
        Size = reader.ReadVector4();
        MeshName = reader.ReadStringPaddedByte(4);

        FieldU1 = reader.ReadInt32();
        FieldU2 = reader.ReadInt32();
        FieldU3 = reader.ReadInt32();
        FieldU4 = reader.ReadInt32();

        // Emitter Animation
        AnimCount = reader.ReadInt32();
        if (AnimCount > 0)
        {
            EmitterAnim = reader.ReadObject<Animation>();
        }

        // ParticleSaveLoadList
        if (ParticleCount > 0)
        {
            // ParticleSaveLoad
            for (int p = 0; p < ParticleCount; p++)
            {
                if (reader.ReadStringPaddedByte(4) == "ParticleChunk")
                {
                    ParticleSaveLoad.Add(reader.ReadObject<Particle>());
                }
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte("EmitterChunk", 4);
        writer.WriteInt32(ParticleCount);
        writer.WriteStringPaddedByte(Name, 4);
        writer.WriteInt32(MaxGenerateCount);
        writer.WriteInt32(GenerateCount);
        writer.WriteInt32(ParticleDataFlags);
        writer.WriteInt32(Infinite ? 1 : 0);
        writer.WriteSingle(InitialEmissionGap);

        writer.WriteVector4(InitialPosition);
        writer.WriteVector4(RotationXYZ);
        writer.WriteVector4(RotationXYZBias);
        writer.WriteVector4(InitialRotationXYZ);
        writer.WriteVector4(InitialRotation);

        writer.WriteSingle(InitialEmitterLifeTime);
        writer.WriteSingle(EmitStartTime);
        writer.WriteInt32(EmitCondition);
        writer.WriteInt32(EmitterType);

        writer.WriteObject(CylinderParams);
        writer.WriteObject(SphereParams);
        writer.WriteVector4(Size);
        writer.WriteStringPaddedByte(MeshName, 4);

        writer.WriteInt32(FieldU1);
        writer.WriteInt32(FieldU2);
        writer.WriteInt32(FieldU3);
        writer.WriteInt32(FieldU4);

        writer.WriteInt32(AnimCount);
        if (AnimCount > 0)
        {
            writer.WriteObject(EmitterAnim);
        }

        for (int i = 0; i < ParticleCount; i++)
        {
            writer.WriteObject(ParticleSaveLoad[i]);
        }
    }
}