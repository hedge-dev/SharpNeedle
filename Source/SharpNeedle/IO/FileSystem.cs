namespace SharpNeedle.IO;

/// <summary>
/// File system that allows for mounting of e.g. virtual directories.
/// </summary>
public class FileSystem
{
    private readonly Dictionary<string, IDirectory> _mountPoints = new(StringComparer.InvariantCultureIgnoreCase);

    public static FileSystem Instance => Singleton.GetInstance<FileSystem>()!;


    static FileSystem()
    {
        FileSystem instance = new();
        Singleton.SetInstance(instance);
    }


    public static string GetPathRoot(ReadOnlySpan<char> path)
    {
        int colonIndex = path.IndexOf(':');
        if (colonIndex >= 0)
        {
            return new string(path[..colonIndex]);
        }

        return string.Empty;
    }

    public static string? GetPathWithoutRoot(ReadOnlySpan<char> path)
    {
        string root = GetPathRoot(path);
        if (string.IsNullOrEmpty(root))
        {
            return null;
        }

        path = path[(root.Length + 1)..];
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] is '/' or '\\')
            {
                continue;
            }

            return new string(path[i..]);
        }

        return null;
    }


    public void Mount(string root, IDirectory dir)
    {
        if (root.IndexOf(':', out int colonIndex))
        {
            root = root[..colonIndex];
        }

        _mountPoints.Add(root, dir);
    }

    public IEnumerable<(string Name, IDirectory Directory)> GetMounts()
    {
        foreach (KeyValuePair<string, IDirectory> mountPoint in _mountPoints)
        {
            yield return (mountPoint.Key, mountPoint.Value);
        }

        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            yield return (GetPathRoot(drive.Name.AsSpan()), new HostDirectory(drive.Name));
        }
    }


    public IFile? Open(string path)
    {
        string root = GetPathRoot(path);
        if (_mountPoints.TryGetValue(root, out IDirectory? dir))
        {
            return dir[GetPathWithoutRoot(path) ?? string.Empty];
        }

        return HostFile.Open(path);
    }

    public IFile Create(string path, bool overwrite = true)
    {
        string root = GetPathRoot(path);
        if (_mountPoints.TryGetValue(root, out IDirectory? dir))
        {
            return dir.CreateFile(path, overwrite);
        }

        return HostFile.Create(path, overwrite);
    }

    public IDirectory CreateDirectory(string path)
    {
        string root = GetPathRoot(path);
        if (_mountPoints.TryGetValue(root, out IDirectory? dir))
        {
            return dir.CreateDirectory(path);
        }

        return HostDirectory.Create(path);
    }
}