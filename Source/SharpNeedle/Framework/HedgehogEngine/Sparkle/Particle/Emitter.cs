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
        reader.ReadStringPaddedByte();
        ParticleCount = reader.ReadInt32();
        Name = reader.ReadStringPaddedByte();

        MaxGenerateCount = reader.ReadInt32();
        GenerateCount = reader.ReadInt32();
        ParticleDataFlags = reader.ReadInt32();
        Infinite = reader.ReadUInt32() == 1;
        InitialEmissionGap = reader.ReadSingle();

        InitialPosition = reader.Read<Vector4>();
        RotationXYZ = reader.Read<Vector4>();
        RotationXYZBias = reader.Read<Vector4>();
        InitialRotationXYZ = reader.Read<Vector4>();
        InitialRotation = reader.Read<Vector4>();

        InitialEmitterLifeTime = reader.ReadSingle();
        EmitStartTime = reader.ReadSingle();
        EmitCondition = reader.ReadInt32();
        EmitterType = reader.ReadInt32();

        CylinderParams = reader.ReadObject<Cylinder>();
        SphereParams = reader.ReadObject<Sphere>();
        Size = reader.Read<Vector4>();
        MeshName = reader.ReadStringPaddedByte();

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
                if (reader.ReadStringPaddedByte() == "ParticleChunk")
                {
                    ParticleSaveLoad.Add(reader.ReadObject<Particle>());
                }
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte("EmitterChunk");
        writer.Write(ParticleCount);
        writer.WriteStringPaddedByte(Name);
        writer.Write(MaxGenerateCount);
        writer.Write(GenerateCount);
        writer.Write(ParticleDataFlags);
        writer.Write(Infinite ? 1 : 0);
        writer.Write(InitialEmissionGap);

        writer.Write(InitialPosition);
        writer.Write(RotationXYZ);
        writer.Write(RotationXYZBias);
        writer.Write(InitialRotationXYZ);
        writer.Write(InitialRotation);

        writer.Write(InitialEmitterLifeTime);
        writer.Write(EmitStartTime);
        writer.Write(EmitCondition);
        writer.Write(EmitterType);

        writer.WriteObject(CylinderParams);
        writer.WriteObject(SphereParams);
        writer.Write(Size);
        writer.WriteStringPaddedByte(MeshName);

        writer.Write(FieldU1);
        writer.Write(FieldU2);
        writer.Write(FieldU3);
        writer.Write(FieldU4);

        writer.Write(AnimCount);
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