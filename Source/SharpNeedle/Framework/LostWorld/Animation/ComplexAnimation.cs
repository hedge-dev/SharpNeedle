namespace SharpNeedle.Framework.LostWorld.Animation;

public class ComplexAnimation : AnimationDef
{
    public List<SimpleAnimation> Animations { get; set; } = [];
    public IComplexData? Data { get; set; }

    public bool HasBlender => Data is BlenderData;
    public BlenderData? Blender => Data as BlenderData;

    public bool HasSequenceTable => Data is SequenceTable;
    public SequenceTable? SequenceTable => Data as SequenceTable;

    public ComplexAnimation() : base(AnimationType.Complex)
    {

    }

    public override void Read(BinaryObjectReader reader)
    {
        base.Read(reader);
        Animations = reader.ReadObject<BinaryList<SimpleAnimation>>();
        reader.ReadOffset(() =>
        {
            ComplexDataType type = reader.Read<ComplexDataType>();
            switch(type)
            {
                case ComplexDataType.SequenceTable:
                    Data = reader.ReadObject<SequenceTable>();
                    break;

                case ComplexDataType.Blender:
                    Data = reader.ReadObject<BlenderData>();
                    break;
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        if(Data == null)
        {
            throw new InvalidOperationException("Data is null!");
        }

        base.Write(writer);
        writer.WriteObject<BinaryList<SimpleAnimation>>(Animations);
        writer.WriteOffset(() => writer.WriteObject(Data, true));
    }
}

public enum ComplexDataType
{
    SequenceTable, Blender
}