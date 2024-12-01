namespace SharpNeedle.IO;

public class FileSystem
{
    private static readonly Dictionary<string, IDirectory> mMountPoints = new(StringComparer.InvariantCultureIgnoreCase);

    public static string GetPathRoot(ReadOnlySpan<char> path)
    {
        int colonIndex = path.IndexOf(':');
        if(colonIndex >= 0)
        {
            return new string(path[..colonIndex]);
        }

        return string.Empty;
    }

    public static string GetPathWithoutRoot(ReadOnlySpan<char> path)
    {
        string root = GetPathRoot(path);
        if(string.IsNullOrEmpty(root))
        {
            return null;
        }

        path = path[(root.Length + 1)..];
        for(int i = 0; i < path.Length; i++)
        {
            if(path[i] is '/' or '\\')
            {
                continue;
            }

            return new string(path[i..]);
        }

        return null;
    }

    public static IEnumerable<(string Name, IDirectory Directory)> GetMounts()
    {
        foreach(KeyValuePair<string, IDirectory> mountPoint in mMountPoints)
        {
            yield return (mountPoint.Key, mountPoint.Value);
        }

        foreach(DriveInfo drive in DriveInfo.GetDrives())
        {
            yield return (GetPathRoot(drive.Name.AsSpan()), new HostDirectory(drive.Name));
        }
    }

    public static void Mount(string root, IDirectory dir)
    {
        if(root.IndexOf(':', out int colonIndex))
        {
            root = root[..colonIndex];
        }

        mMountPoints.Add(root, dir);
    }

    public static IFile Open(string path)
    {
        string root = GetPathRoot(path);
        if(mMountPoints.TryGetValue(root, out IDirectory dir))
        {
            return dir[GetPathWithoutRoot(path)];
        }

        return HostFile.Open(path);
    }

    public static IFile Create(string path)
    {
        string root = GetPathRoot(path);
        if(mMountPoints.TryGetValue(root, out IDirectory dir))
        {
            return dir.CreateFile(path);
        }

        return HostFile.Create(path);
    }

    public static IDirectory CreateDirectory(string path)
    {
        string root = GetPathRoot(path);
        if(mMountPoints.TryGetValue(root, out IDirectory dir))
        {
            return dir.CreateDirectory(path);
        }

        return HostDirectory.Create(path);
    }
}