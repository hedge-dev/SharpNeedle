namespace SharpNeedle.HedgehogEngine;

using System.IO;

[NeedleResource(ResourceId)]
public class ArchiveList : ResourceBase, IDirectory, IStreamable
{
    public const string ResourceId = "hh/archive-list";
    public static uint Signature { get; } = BinaryHelper.MakeSignature<uint>("ARL2");

    public long MaxSplitSize { get; set; } = 1024 * 1024 * 1024;
    public IDirectory Parent { get; private set; }
    public bool DependsResolved { get; private set; }
    public List<Archive> Archives { get; private set; }
    public List<uint> ArchiveSizes { get; set; } = new();
    public HashSet<string> Files { get; set; } = new();
    public string Path { get; set; }
    public IFile this[string name] => GetFile(name);

    public bool DeleteFile(string name)
    {
        foreach (var archive in Archives)
        {
            if (archive.DeleteFile(name))
                return true;
        }

        return false;
    }

    // Format doesn't support directories
    public bool DeleteDirectory(string name) => false;
    public IDirectory OpenDirectory(string name) => null;
    public IDirectory CreateDirectory(string name) => null;

    public IFile CreateFile(string name)
    {
        if (Archives == null)
        {
            Archives = new List<Archive>
            {
                new ()
            };

            EnsureSplit();
            return Archives.First().CreateFile(name);
        }

        return Archives.Last().CreateFile(name);
    }

    public IFile Add(IFile file)
    {
        if (Archives == null)
        {
            Archives = new List<Archive>
            {
                new ()
            };

            return Archives.First().Add(file);
        }

        EnsureSplit();
        return Archives.Last().Add(file);
    }

    public IEnumerable<IDirectory> GetDirectories() => Enumerable.Empty<IDirectory>();

    private void EnsureSplit()
    {
        if (Archives.Last().CalculateFileSize() <= MaxSplitSize)
            return;

        Archives.Add(new Archive());
    }

    public override void Read(IFile file)
    {
        Name = file.Name;
        Parent = file.Parent;
        Path = System.IO.Path.Combine(Parent.Path, Name);

        using var stream = file.Open();
        using var reader = new BinaryValueReader(stream, StreamOwnership.Retain, Endianness.Little);

        var sig = reader.ReadUInt32();
        if (sig != Signature)
            throw new Exception("Invalid ARL file");
            
        var count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            ArchiveSizes.Add(reader.ReadUInt32());
        }

        while (stream.Position < stream.Length)
        {
            var name = reader.ReadString(StringBinaryFormat.PrefixedLength8);
                
            if (!Files.Contains(name))
                Files.Add(name);
        }
    }

    public override IEnumerable<ResourceDependency> GetDependencies()
    {
        var baseName = System.IO.Path.GetFileNameWithoutExtension(Name);

        for (int i = 0; i < ArchiveSizes.Count; i++)
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
        using var writer = new BinaryValueWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);

        writer.Write(Signature);
        if (Archives == null)
        {
            writer.Write(ArchiveSizes.Count);
            foreach (var size in ArchiveSizes)
                writer.Write(size);

            foreach (var name in Files)
                writer.WriteString(StringBinaryFormat.PrefixedLength8, name);
        }
        else
        {
            writer.Write(Archives.Count);
            foreach (var archive in Archives)
                writer.Write((uint)archive.CalculateFileSize());

            foreach (var aFile in this)
                writer.WriteString(StringBinaryFormat.PrefixedLength8, aFile.Name);
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if (ArchiveSizes.Count > 0)
            Archives = new List<Archive>(ArchiveSizes.Count);

        var baseName = System.IO.Path.GetFileNameWithoutExtension(Name);
        for (int i = 0; i < ArchiveSizes.Count; i++)
        {
            var name = $"{baseName}.ar.{i:00}";
            Archives.Add(resolver.Open<Archive>(name));
        }
        
        DependsResolved = true;
    }

    public override void WriteDependencies(IDirectory dir)
    {
        var baseName = System.IO.Path.GetFileNameWithoutExtension(Name);
        for (var i = 0; i < Archives.Count; i++)
        {
            var archive = Archives[i];
            using var file = dir.CreateFile($"{baseName}.ar.{i:00}");
            archive.Write(file);
        }
    }

    [ResourceCheckFunction]
    public static bool IsResourceValid(IFile file)
    {
        using var stream = file.Open();
        return BitConverter.ToUInt32(stream.ReadBytes(sizeof(uint)), 0) == Signature;
    }

    public IFile GetFile(string name)
    {
        foreach (var archive in Archives)
        {
            var file = archive.GetFile(name);
            if (file != null)
                return file;
        }

        return null;
    }

    public void LoadToMemory()
    {
        foreach (var archive in Archives)
            archive.LoadToMemory();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        if (Archives != null)
        {
            foreach (var archive in Archives)
                archive.Dispose();
        }
    }

    public IEnumerator<IFile> GetEnumerator()
    {
        if (Archives == null)
            yield break;

        foreach (var archive in Archives)
        {
            foreach (var file in archive)
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