namespace SharpNeedle.IO;
using System.IO;

public class FileSystem
{
    private static readonly Dictionary<string, IDirectory> mMountPoints = new(StringComparer.InvariantCultureIgnoreCase);

    public static string GetPathRoot(ReadOnlySpan<char> path)
    {
        var colonIndex = path.IndexOf(':');
        if (colonIndex >= 0)
            return new string(path[..colonIndex]);
            
        return string.Empty;
    }

    public static string GetPathWithoutRoot(ReadOnlySpan<char> path)
    {
        var root = GetPathRoot(path);
        if (string.IsNullOrEmpty(root))
            return null;

        path = path[(root.Length + 1)..];
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == '/' || path[i] == '\\')
                continue;

            return new string(path[i..]);
        }

        return null;
    }

    public static IEnumerable<(string Name, IDirectory Directory)> GetMounts()
    {
        foreach (var mountPoint in mMountPoints)
        {
            yield return (mountPoint.Key, mountPoint.Value);
        }

        foreach (var drive in DriveInfo.GetDrives())
        {
            yield return (GetPathRoot(drive.Name.AsSpan()), new HostDirectory(drive.Name));
        }
    }

    public static void Mount(string root, IDirectory dir)
    {
        if (root.IndexOf(':', out var colonIndex))
            root = root.Substring(0, colonIndex);

        mMountPoints.Add(root, dir);
    }

    public static IFile Open(string path)
    {
        var root = GetPathRoot(path);
        if (mMountPoints.TryGetValue(root, out var dir))
        {
            return dir[GetPathWithoutRoot(path)];
        }

        return HostFile.Open(path);
    }

    public static IFile Create(string path)
    {
        var root = GetPathRoot(path);
        if (mMountPoints.TryGetValue(root, out var dir))
        {
            return dir.CreateFile(path);
        }

        return HostFile.Create(path);
    }

    public static IDirectory CreateDirectory(string path)
    {
        var root = GetPathRoot(path);
        if (mMountPoints.TryGetValue(root, out var dir))
        {
            return dir.CreateDirectory(path);
        }

        return HostDirectory.Create(path);
    }
}