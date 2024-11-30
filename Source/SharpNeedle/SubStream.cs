namespace SharpNeedle;
using System.IO;

public class SubStream : Stream
{
    public Stream BaseStream { get; }
    private long Begin { get; }
    private long End { get; }

    private long mPosition;

    public override bool CanRead => BaseStream.CanRead;
    public override bool CanSeek => BaseStream.CanSeek;
    public override bool CanWrite => BaseStream.CanWrite;
    public override long Length => End - Begin;

    public override long Position
    {
        get => mPosition;
        set
        {
            BaseStream.Position = Begin + value;
            mPosition = value;
        }
    }

    public SubStream(Stream stream, long begin, long length)
    {
        BaseStream = stream;
        Begin = begin;
        End = begin + length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsurePosition();
        if(Position >= Length)
        {
            return 0;
        }

        if(Position + count >= Length)
        {
            count = (int)(Length - Position);
        }

        int bytesRead = BaseStream.Read(buffer, offset, count);
        mPosition += bytesRead;

        return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsurePosition();
        mPosition += count;
        BaseStream.Write(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch(origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;

            case SeekOrigin.Current:
                Position += offset;
                EnsurePosition();
                break;

            case SeekOrigin.End:
                Position = Length - offset;
                break;
        }

        return Position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void EnsurePosition()
    {
        if(BaseStream.Position - Begin != mPosition)
        {
            Position = mPosition;
        }
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Flush()
    {
        BaseStream.Flush();
    }
}