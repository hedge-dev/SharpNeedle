namespace SharpNeedle.IO;

public class HostFile : IFile
{
    private DateTime _lastModified;


    protected FileAccess Access { get; set; }

    protected FileStream? BaseStream { get; set; }


    public IDirectory Parent 
        => HostDirectory.ParentFromPath(Path) 
        ?? throw new IOException("Could not find directory for file!");

    public string Path { get; }

    public string Name { get; set; }

    public long Length { get; }

    public DateTime LastModified
    {
        get => _lastModified;
        set
        {
            _lastModified = value;
            File.SetLastWriteTime(Path, value);
        }
    }


    public static HostFile Create(string fullPath, bool overwrite = true)
    {
        if(File.Exists(fullPath) && !overwrite)
        {
            throw new InvalidOperationException($"File \"{fullPath}\" already exists! Create failed!");
        }

        File.WriteAllBytes(fullPath, []);
        return new HostFile(fullPath);
    }

    public static HostFile? Open(string fullPath)
    {
        return !File.Exists(fullPath) ? null : new HostFile(fullPath);
    }

    public static bool Delete(string fullPath)
    {
        if(!File.Exists(fullPath))
        {
            return false;
        }

        try
        {
            File.Delete(fullPath);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public HostFile(string filepath)
    {
        if(!File.Exists(filepath))
        {
            throw new FileNotFoundException(filepath);
        }

        FileInfo info = new(filepath);
        Name = info.Name;
        Path = filepath;
        Length = info.Length;
        LastModified = info.LastWriteTime;
    }


    public void Dispose()
    {
        BaseStream?.Dispose();
        BaseStream = null;
    }

    public bool CheckDisposed()
    {
        if(BaseStream == null)
        {
            return false;
        }

        try
        {
            _ = BaseStream.Length;
            return false;
        }
        catch
        {
            return true;
        }
    }

    public Stream Open(FileAccess access = FileAccess.Read)
    {
        if(CheckDisposed())
        {
            BaseStream = null;
        }

        if(BaseStream != null)
        {
            if(Access == access)
            {
                return BaseStream;
            }
            else
            {
                Dispose();
            }
        }

        Access = access;
        BaseStream = new FileStream(Path, FileHelper.FileAccessToMode(Access), Access, FileShare.ReadWrite);
        return BaseStream;
    }


    protected bool Equals(HostFile other)
    {
        return Path == other.Path;
    }

    public override bool Equals(object? obj)
    {
        return obj is HostFile file && Equals(file);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Length, Path);
    }

    public override string ToString()
    {
        return Name;
    }

}