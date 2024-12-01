namespace SharpNeedle.IO;

public class WrappedStream<TStream> : Stream where TStream : Stream
{
    public TStream BaseStream { get; }

    public bool Disposed { get; private set; }
    public override bool CanRead => BaseStream.CanRead;
    public override bool CanSeek => BaseStream.CanSeek;
    public override bool CanWrite => BaseStream.CanWrite;
    public override long Length => BaseStream.Length;
    public bool KeepOpen { get; set; }

    public override long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    public event Action<TStream>? OnClosing;

    public WrappedStream(TStream baseStream, bool keepOpen)
    {
        BaseStream = baseStream;
        KeepOpen = keepOpen;
    }

    public WrappedStream(TStream baseStream) : this(baseStream, true)
    {

    }

    public override void Flush()
    {
        BaseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return BaseStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return BaseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        BaseStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        BaseStream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        if(!disposing || Disposed)
        {
            return;
        }

        Disposed = true;
        OnClosing?.Invoke(BaseStream);

        if(!KeepOpen)
        {
            BaseStream.Dispose();
        }
    }
}