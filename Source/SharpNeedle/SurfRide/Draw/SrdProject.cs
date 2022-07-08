namespace SharpNeedle.SurfRide.Draw;
using System.IO;

[NeedleResource("sr/project", @"\.swif$")]
public class SrdProject : ResourceBase, IBinarySerializable<uint>
{
    public ProjectChunk Project { get; set; }
    public TextureListChunk TextureLists { get; set; }
    public int RevisionDate { get; set; }
    public Endianness Endianness { get; set; } = BinaryHelper.PlatformEndianness;

    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;
        
        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        Read(reader, 2);
    }
    
    public override void Write(IFile file)
    {
        Name = file.Name;
        BaseFile = file;
        
        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness);
        Write(writer, 2);
    }

    public void Read(BinaryObjectReader reader, uint version)
    {
        var info = reader.ReadObject<InfoChunk, ChunkBinaryOptions>(new() { Version = version });
        foreach (var chunk in info.Chunks)
        {
            switch (chunk)
            {
                case ProjectChunk project:
                    Project = project;
                    break;
                case TextureListChunk tl:
                    TextureLists = tl;
                    break;
            }
        }
        
        RevisionDate = info.RevisionDate;
        Endianness = reader.Endianness;
    }
    
    public void Write(BinaryObjectWriter writer, uint version)
    {
        var info = new InfoChunk();
        info.RevisionDate = RevisionDate;
        info.Chunks.Add(TextureLists);
        info.Chunks.Add(Project);
        writer.WriteObject(info, new ChunkBinaryOptions() { Version = version });
    }

    public TextureList GetTextureList(string name)
    {
        return TextureLists.Find(item => item.Name == name);
    }
}
