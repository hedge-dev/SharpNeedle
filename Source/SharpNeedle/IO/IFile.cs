namespace SharpNeedle.IO;
using System.IO;

public interface IFile : IDisposable
{
    IDirectory Parent { get; }
    string Name { get; set; }
    string Path { get; }
    long Length { get; }
    DateTime LastModified { get; set; }

    Stream Open(FileAccess access = FileAccess.Read);
}