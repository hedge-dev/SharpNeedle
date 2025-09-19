namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

[NeedleResource("hh/sparkle", @"\.part-bin$")]
public class SparkleProject : ResourceBase, IBinarySerializable
{
    public ProjectInfo? ProjectInfo;
    public Effect? Effect;
    public Material? Material;
    public List<Emitter> Emitters = [];

    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;

        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        Read(reader);
    }
    public override void Write(IFile file)
    {
        Name = file.Name;
        BaseFile = file;

        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);
        Write(writer);
    }
    public void Read(BinaryObjectReader reader)
    {
        ProjectInfo = reader.ReadObject<ProjectInfo>();

        switch (ProjectInfo.Type)
        {
            case ProjectType.Effect:
            {
                //InportExportEffect
                Effect = reader.ReadObject<Effect>();
                for (int i = 0; i < ProjectInfo.EmitterCount; i++)
                {
                    Emitters.Add(reader.ReadObject<Emitter>());
                }
                break;
            }
            case ProjectType.Material:
            {
                //InportExportMaterial
                Material = reader.ReadObject<Material>();
                break;
            }
        }
    }
    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteObject(ProjectInfo);

        if (ProjectInfo.Type == ProjectType.Effect)
        {
            writer.WriteObject(Effect);
            for (int i = 0; i < ProjectInfo.EmitterCount; i++)
            {
                writer.WriteObject(Emitters[i]);
            }
        }

        if (ProjectInfo.Type == ProjectType.Material)
            writer.WriteObject(Material);

        //S E G A
        writer.Write(83);
        writer.Write(69);
        writer.Write(71);
        writer.Write(65);
    }
}