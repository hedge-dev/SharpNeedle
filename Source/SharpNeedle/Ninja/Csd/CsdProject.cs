namespace SharpNeedle.Ninja.Csd;
using System.IO;

[NeedleResource("nn/csd-project", @"\.[xy]ncp$")]
public class CsdProject : ResourceBase, IBinarySerializable
{
    public CsdPackage Package { get; set; }
    public ProjectChunk Project { get; set; }
    public TextureListChunk Textures { get; set; }
    public Endianness Endianness { get; set; }

    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;
        
        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        Read(reader);
    }

    public override void Write(IFile file)
    {
        Name = file.Name;
        BaseFile = file;

        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness);
        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        Package = reader.ReadObject<CsdPackage>();
        Endianness = Package.Endianness;
        for (int i = 0; i < Package.Files.Count; i++)
        {
            var stream = Package.GetStream(i);
            using var infoReader = new BinaryObjectReader(stream, StreamOwnership.Transfer, Package.Endianness);
            try
            {
                var info = infoReader.ReadObject<InfoChunk>();
                foreach (var chunk in info.Chunks)
                {
                    switch (chunk)
                    {
                        case ProjectChunk project:
                            Project = project;
                            break;

                        case TextureListChunk tl:
                            Textures = tl;
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                if (e is not InvalidDataException)
                    throw;
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        Package = new CsdPackage
        {
            Endianness = Endianness
        };
        
        CreatePackageFile(Project);

        if (Textures != null)
            CreatePackageFile(Textures);

        writer.WriteObject(Package);

        void CreatePackageFile(IChunk chunk)
        {
            using var stream = Package.GetStream(Package.Add(), true, true);
            using var infoWriter = new BinaryObjectWriter(stream, StreamOwnership.Retain, Endianness);
            var info = new InfoChunk
            {
                Signature = BinaryHelper.MakeSignature<uint>(Endianness == Endianness.Little ? "NXIF" : "NYIF"),
            };
            info.Chunks.Add(chunk);
            infoWriter.WriteObject(info);
        }
    }
}