namespace SharpNeedle.Framework.Ninja.Csd;

[NeedleResource("nn/csd-project", @"\.[gsxy]ncp$")]
public class CsdProject : ResourceBase, IBinarySerializable
{
    public CsdPackage? Package { get; set; }
    public ProjectChunk? Project { get; set; }
    public ITextureList? Textures { get; set; }
    public Endianness Endianness { get; set; }

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

        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness);
        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        Package = reader.ReadObject<CsdPackage>();
        Endianness = Package.Endianness;
        ChunkBinaryOptions options = new();
        for (int i = 0; i < Package.Files.Count; i++)
        {
            if (Package.Files[i].Length == 0)
                continue;
            Stream stream = Package.GetStream(i);
            using BinaryObjectReader infoReader = new(stream, StreamOwnership.Transfer, Package.Endianness);
            try
            {
                InfoChunk info = infoReader.ReadObject<InfoChunk, ChunkBinaryOptions>(options);
                foreach (IChunk chunk in info.Chunks)
                {
                    switch (chunk)
                    {
                        case ProjectChunk project:
                            Project = project;
                            options.TextureFormat = project.TextureFormat;
                            break;

                        case ITextureList tl:
                            Textures = tl;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is not InvalidDataException)
                {
                    throw;
                }
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        if (Project == null)
        {
            throw new InvalidOperationException("Project is null!");
        }

        Package = new CsdPackage
        {
            Endianness = Endianness
        };

        if (Textures is TextureListMirage)
        {
            Project.TextureFormat = TextureFormat.Mirage;
        }
        else if (Textures is TextureListNN)
        {
            Project.TextureFormat = TextureFormat.NextNinja;
        }

        CreatePackageFile(Project);

        if (Textures != null)
        {
            CreatePackageFile(Textures);
        }

        writer.WriteObject(Package);

        void CreatePackageFile(IChunk chunk)
        {
            using Stream stream = Package.GetStream(Package.Add(), true, true);
            using BinaryObjectWriter infoWriter = new(stream, StreamOwnership.Retain, Endianness);
            InfoChunk info = new()
            {
                Signature = BinaryHelper.MakeSignature<uint>(Endianness == Endianness.Little ? "NXIF" : "NYIF"),
            };
            info.Chunks.Add(chunk);
            infoWriter.WriteObject(info);
        }
    }
}