namespace SharpNeedle.IO;
using System.IO;

public class HostDirectory : IDirectory
{
    string IDirectory.Path => FullPath;
    public string FullPath { get; set; }

    public IDirectory Parent
        => FromPath(Path.GetDirectoryName(Path.IsPathRooted(FullPath.AsSpan())
            ? FullPath
            : Path.TrimEndingDirectorySeparator(FullPath)));

    public string Name
    {
        get
        {
            string name = Path.GetFileName(FullPath);
            if(string.IsNullOrEmpty(name))
            {
                return FullPath;
            }

            return name;
        }

        set => throw new NotSupportedException();
    }

    public IFile this[string name] => GetFile(name);

    public IDirectory OpenDirectory(string name)
    {
        return FromPath(Path.Combine(FullPath, name));
    }

    public bool DeleteFile(string name)
    {
        string path = Path.Combine(FullPath, name);
        if(!File.Exists(path))
        {
            return false;
        }

        try
        {
            File.Delete(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteDirectory(string name)
    {
        string path = Path.Combine(FullPath, name);
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

    public static HostDirectory FromPath(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            return null;
        }

        if(!Directory.Exists(path))
        {
            return null;
        }

        return new HostDirectory
        {
            FullPath = path
        };
    }

    private HostDirectory()
    {

    }

    public HostDirectory(string path)
    {
        FullPath = Path.GetFullPath(path);
        if(!Directory.Exists(FullPath))
        {
            throw new DirectoryNotFoundException(path);
        }
    }

    public IFile GetFile(string name)
    {
        string path = Path.Combine(FullPath, name);

        return File.Exists(path) ? new HostFile(path) : null;
    }

    public IFile CreateFile(string name)
    {
        return HostFile.Create(Path.Combine(FullPath, name));
    }

    public IDirectory CreateDirectory(string name)
    {
        return Create(Path.Combine(FullPath, Name));
    }

    public IFile Add(IFile file)
    {
        HostFile destFile = (HostFile)CreateFile(file.Name);
        Stream destStream = destFile.Open(FileAccess.Write);
        Stream srcStream = file.Open();
        srcStream.CopyTo(destStream);

        destStream.Dispose();
        destFile.LastModified = file.LastModified;
        return destFile;
    }

    public static HostDirectory Create(string path)
    {
        Directory.CreateDirectory(path);
        return new HostDirectory(path);
    }

    public IEnumerable<IDirectory> GetDirectories()
    {
        foreach(string file in Directory.EnumerateDirectories(FullPath))
        {
            yield return new HostDirectory(file);
        }
    }

    public IEnumerator<IFile> GetEnumerator()
    {
        foreach(string file in Directory.EnumerateFiles(FullPath))
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
}