namespace SharpNeedle.IO;

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

        return new HostFile(path);
    }

    public static IFile Create(string path)
    {
        var root = GetPathRoot(path);
        if (mMountPoints.TryGetValue(root, out var dir))
        {
            return dir.Create(path);
        }

        return HostFile.Create(path);
    }
}