namespace SharpNeedle.IO;

public class HostDirectory : IDirectory
{
    public IDirectory? Parent => ParentFromPath(Path);

    public string Path { get; set; }

    public string Name
    {
        get
        {
            string name = System.IO.Path.GetFileName(Path);
            if(string.IsNullOrEmpty(name))
            {
                return Path;
            }

            return name;
        }

        set => throw new NotSupportedException();
    }

    public IFile? this[string name] => GetFile(name);


    internal static HostDirectory? ParentFromPath(string path)
    {
        if(!System.IO.Path.IsPathRooted(path.AsSpan()))
        {
            path = System.IO.Path.TrimEndingDirectorySeparator(path);
        }

        string? directoryName = System.IO.Path.GetDirectoryName(path);
        return FromPath(directoryName);
    }

    public static HostDirectory? FromPath(string? path)
    {
        if(string.IsNullOrEmpty(path)
            || !Directory.Exists(path))
        {
            return null;
        }

        return new HostDirectory
        {
            Path = path
        };
    }

    public static HostDirectory Create(string path)
    {
        Directory.CreateDirectory(path);
        return new HostDirectory(path);
    }


    private HostDirectory()
    {
        Path = string.Empty;
    }

    public HostDirectory(string path)
    {
        Path = System.IO.Path.GetFullPath(path);
        if(!Directory.Exists(Path))
        {
            throw new DirectoryNotFoundException(path);
        }
    }


    public IEnumerable<IDirectory> GetDirectories()
    {
        foreach(string file in Directory.EnumerateDirectories(Path))
        {
            yield return new HostDirectory(file);
        }
    }

    public IDirectory? GetDirectory(string name)
    {
        return FromPath(System.IO.Path.Combine(Path, name));
    }

    public IDirectory CreateDirectory(string name)
    {
        string directoryPath = System.IO.Path.Combine(Path, Name);

        if(Directory.Exists(directoryPath))
        {
            throw new InvalidOperationException($"Failed to create directory \"{name}\"! Directory \"{Path}\" already has a directory with the same name!");
        }

        return Create(System.IO.Path.Combine(Path, Name));
    }

    public bool DeleteDirectory(string name)
    {
        string path = System.IO.Path.Combine(Path, name);
        if(!Directory.Exists(path))
        {
            return false;
        }

        try
        {
            Directory.Delete(path, true);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public IFile? GetFile(string name)
    {
        string path = System.IO.Path.Combine(Path, name);

        return File.Exists(path) ? new HostFile(path) : null;
    }

    public IFile CreateFile(string name, bool overwrite = true)
    {
        return HostFile.Create(System.IO.Path.Combine(Path, name), overwrite);
    }
     
    public IFile AddFile(IFile file, bool overwrite = true)
    {
        HostFile destFile = (HostFile)CreateFile(file.Name, overwrite);
        Stream destStream = destFile.Open(FileAccess.Write);
        Stream srcStream = file.Open();
        srcStream.CopyTo(destStream);

        destStream.Dispose();
        destFile.LastModified = file.LastModified;
        return destFile;
    }

    public bool DeleteFile(string name)
    {
        return HostFile.Delete(System.IO.Path.Combine(Path, name));
    }


    public IEnumerator<IFile> GetEnumerator()
    {
        foreach(string file in Directory.EnumerateFiles(Path))
        {
            yield return new HostFile(file);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is HostDirectory directory &&
               Path == directory.Path;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Path);
    }
}