using System;
using System.IO;

namespace SharpNeedle.IO
{
    public interface IFile : IDisposable
    {
        IDirectory Parent { get; }
        string Name { get; set; }
        string Path { get; }
        long Length { get; }
        Stream Open(FileAccess access = FileAccess.Read);
    }
}
