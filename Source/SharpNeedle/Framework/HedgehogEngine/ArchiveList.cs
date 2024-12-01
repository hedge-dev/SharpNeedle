namespace SharpNeedle.Framework.HedgehogEngine;

[NeedleResource(ResourceId)]
public class ArchiveList : ResourceBase, IDirectory, IStreamable
{
    public const string ResourceId = "hh/archive-list";
    public static uint Signature { get; } = BinaryHelper.MakeSignature<uint>("ARL2");

    public long MaxSplitSize { get; set; } = 1024 * 1024 * 1024;
    public IDirectory Parent { get; private set; }
    public bool DependsResolved { get; private set; }
    public List<Archive> Archives { get; private set; }
    public List<uint> ArchiveSizes { get; set; } = [];
    public HashSet<string> Files { get; set; } = [];
    public string Path { get; set; }
    public IFile this[string name] => GetFile(name);

    public bool DeleteFile(string name)
    {
        foreach(Archive archive in Archives)
        {
            if(archive.DeleteFile(name))
            {
                return true;
            }
        }

        return false;
    }

    // Format doesn't support directories
    public bool DeleteDirectory(string name)
    {
        return false;
    }

    public IDirectory OpenDirectory(string name)
    {
        return null;
    }

    public IDirectory CreateDirectory(string name)
    {
        return null;
    }

    public IFile CreateFile(string name)
    {
        if(Archives == null)
        {
            Archives =
            [
                []
            ];

            EnsureSplit();
            return Archives.First().CreateFile(name);
        }

        return Archives.Last().CreateFile(name);
    }

    public IFile Add(IFile file)
    {
        if(Archives == null)
        {
            Archives =
            [
                []
            ];

            return Archives.First().Add(file);
        }

        EnsureSplit();
        return Archives.Last().Add(file);
    }

    public IEnumerable<IDirectory> GetDirectories()
    {
        return [];
    }

    private void EnsureSplit()
    {
        if(Archives.Last().CalculateFileSize() <= MaxSplitSize)
        {
            return;
        }

        Archives.Add([]);
    }

    public override void Read(IFile file)
    {
        Name = file.Name;
        Parent = file.Parent;
        Path = System.IO.Path.Combine(Parent.Path, Name);

        using Stream stream = file.Open();
        using BinaryValueReader reader = new(stream, StreamOwnership.Retain, Endianness.Little);

        uint sig = reader.ReadUInt32();
        if(sig != Signature)
        {
            throw new Exception("Invalid ARL file");
        }

        int count = reader.ReadInt32();
        for(int i = 0; i < count; i++)
        {
            ArchiveSizes.Add(reader.ReadUInt32());
        }

        while(stream.Position < stream.Length)
        {
            string name = reader.ReadString(StringBinaryFormat.PrefixedLength8);

            if(!Files.Contains(name))
            {
                Files.Add(name);
            }
        }
    }

    public override IEnumerable<ResourceDependency> GetDependencies()
    {
        string baseName = System.IO.Path.GetFileNameWithoutExtension(Name);

        for(int i = 0; i < ArchiveSizes.Count; i++)
        {
            yield return new ResourceDependency()
            {
                Name = $"{baseName}.ar.{i:00}",
                Id = Archive.ResourceId
            };
        }
    }

    public override void Write(IFile file)
    {
        Name = file.Name;
        using BinaryValueWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);

        writer.Write(Signature);
        if(Archives == null)
        {
            writer.Write(ArchiveSizes.Count);
            foreach(uint size in ArchiveSizes)
            {
                writer.Write(size);
            }

            foreach(string name in Files)
            {
                writer.WriteString(StringBinaryFormat.PrefixedLength8, name);
            }
        }
        else
        {
            writer.Write(Archives.Count);
            foreach(Archive archive in Archives)
            {
                writer.Write((uint)archive.CalculateFileSize());
            }

            foreach(IFile aFile in this)
            {
                writer.WriteString(StringBinaryFormat.PrefixedLength8, aFile.Name);
            }
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if(ArchiveSizes.Count > 0)
        {
            Archives = new List<Archive>(ArchiveSizes.Count);
        }

        string baseName = System.IO.Path.GetFileNameWithoutExtension(Name);
        for(int i = 0; i < ArchiveSizes.Count; i++)
        {
            string name = $"{baseName}.ar.{i:00}";
            Archives.Add(resolver.Open<Archive>(name));
        }

        DependsResolved = true;
    }

    public override void WriteDependencies(IDirectory dir)
    {
        string baseName = System.IO.Path.GetFileNameWithoutExtension(Name);
        for(int i = 0; i < Archives.Count; i++)
        {
            Archive archive = Archives[i];
            using IFile file = dir.CreateFile($"{baseName}.ar.{i:00}");
            archive.Write(file);
        }
    }

    [ResourceCheckFunction]
    public static bool IsResourceValid(IFile file)
    {
        using Stream stream = file.Open();
        return BitConverter.ToUInt32(stream.ReadBytes(sizeof(uint)), 0) == Signature;
    }

    public IFile GetFile(string name)
    {
        foreach(Archive archive in Archives)
        {
            IFile file = archive.GetFile(name);
            if(file != null)
            {
                return file;
            }
        }

        return null;
    }

    public void LoadToMemory()
    {
        foreach(Archive archive in Archives)
        {
            archive.LoadToMemory();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if(!disposing)
        {
            return;
        }

        if(Archives != null)
        {
            foreach(Archive archive in Archives)
            {
                archive.Dispose();
            }
        }
    }

    public IEnumerator<IFile> GetEnumerator()
    {
        if(Archives == null)
        {
            yield break;
        }

        foreach(Archive archive in Archives)
        {
            foreach(IFile file in archive)
            {
                yield return file;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}