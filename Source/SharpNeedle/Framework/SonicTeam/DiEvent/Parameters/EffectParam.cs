namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

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

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.ReadMatrix4x4();
        Field40 = reader.ReadInt32();
        Name = reader.ReadDiString(64);
        Field84 = reader.ReadInt32();
        Field88 = reader.ReadInt32();
        Field8C = reader.ReadInt32();
        Field90 = reader.ReadInt32();
        Field94 = reader.ReadInt32();
        Field98 = reader.ReadInt32();
        Field9C = reader.ReadInt32();
        FieldA0 = reader.ReadInt32();
        reader.ReadArray<float>(128, AnimationData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteMatrix4x4(LocalTransform);
        writer.WriteInt32(Field40);
        writer.WriteDiString(Name);
        writer.WriteInt32(Field84);
        writer.WriteInt32(Field88);
        writer.WriteInt32(Field8C);
        writer.WriteInt32(Field90);
        writer.WriteInt32(Field94);
        writer.WriteInt32(Field98);
        writer.WriteInt32(Field9C);
        writer.WriteInt32(FieldA0);
        writer.WriteArrayFixedLength(AnimationData, 128);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.Effect; }
}