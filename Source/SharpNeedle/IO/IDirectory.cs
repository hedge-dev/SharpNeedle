namespace SharpNeedle.IO;

public interface IDirectory : IEnumerable<IFile>
{
    IDirectory Parent { get; }

    string Name { get; set; }
    string Path { get; }

    IFile this[string name] { get; }

    IDirectory OpenDirectory(string name);
    bool DeleteFile(string name);
    bool DeleteDirectory(string name);
    IFile CreateFile(string name);
    IDirectory CreateDirectory(string name);
    IFile Add(IFile file);

    IEnumerable<IDirectory> GetDirectories();
}