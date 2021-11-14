using System.IO;

namespace SharpNeedle.IO
{
    public class StreamFile : IFile
    {
        public Stream BaseStream { get; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; }
        public long Length => BaseStream.Length;
        public IDirectory Parent { get; set; }

        private readonly bool mLeaveOpen;

        public StreamFile(Stream stream)
        {
            BaseStream = stream;
        }

        public StreamFile(Stream stream, bool leaveOpen) : this(stream)
        {
            mLeaveOpen = leaveOpen;
        }

        public StreamFile(Stream stream, string path) : this(stream)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }

        public StreamFile(Stream stream, string path, bool leaveOpen) : this(stream, path)
        {
            mLeaveOpen = leaveOpen;
        }

        public Stream Open(FileAccess access = FileAccess.Read)
        {
            return BaseStream;
        }

        public void Dispose()
        {
            if (!mLeaveOpen)
                BaseStream.Dispose();
        }
    }
}
