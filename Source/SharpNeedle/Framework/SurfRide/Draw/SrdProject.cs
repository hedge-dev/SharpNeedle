namespace SharpNeedle.Framework.SurfRide.Draw;

[NeedleResource("sr/project", @"\.swif$")]
public class SrdProject : ResourceBase, IBinarySerializable<uint>
{
    public ProjectChunk? Project { get; set; }
    public TextureListChunk? TextureLists { get; set; }
    public int Version { get; set; }
    public Endianness Endianness { get; set; } = BinaryHelper.PlatformEndianness;

    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;

        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        Read(reader, 2);
    }

    public override void Write(IFile file)
    {
        Name = file.Name;
        BaseFile = file;

        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness);
        Write(writer, 2);
    }

    public void Read(BinaryObjectReader reader, uint version)
    {
        InfoChunk info = reader.ReadObject<InfoChunk, ChunkBinaryOptions>(new() { Version = version });
        foreach(IChunk chunk in info.Chunks)
        {
            switch(chunk)
            {
                case ProjectChunk project:
                    Project = project;
                    break;
                case TextureListChunk tl:
                    TextureLists = tl;
                    break;
            }
        }

        Version = info.Version;
        Endianness = reader.Endianness;
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        InfoChunk info = new()
        {
            Version = Version
        };

        if(TextureLists != null)
        {
            info.Chunks.Add(TextureLists);
        }

        if(Project != null)
        {
            info.Chunks.Add(Project);
        }

        writer.WriteObject(info, new ChunkBinaryOptions() { Version = version });
    }

    public TextureList? GetTextureList(string name)
    {
        return TextureLists?.Find(item => item.Name == name);
    }
}
