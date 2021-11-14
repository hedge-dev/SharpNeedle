namespace SharpNeedle.IO;

public interface IDirectory : IEnumerable<IFile>
{
    IDirectory Parent { get; }

    string Name { get; set; }

    IFile this[string name] { get; }

    bool DeleteFile(string name);
    bool DeleteDirectory(string name);
    IFile Create(string name);
    IFile Add(IFile file);

    IEnumerable<IDirectory> GetDirectories();
}