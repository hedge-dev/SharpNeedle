namespace SharpNeedle.IO;

public class SubStream : Stream
{
    private long _position;

    /// <summary>
    /// Stream of which this stream is a substream of.
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// Start position of the substream in <see cref="BaseStream"/>.
    /// </summary>
    public long Begin { get; }

    /// <summary>
    /// End position of the substream in <see cref="BaseStream"/>.
    /// </summary>
    public long End { get; }

    public override bool CanRead => BaseStream.CanRead;
    public override bool CanSeek => BaseStream.CanSeek;
    public override bool CanWrite => BaseStream.CanWrite;
    public override long Length => End - Begin;

    public override long Position
    {
        get => _position;
        set
        {
            BaseStream.Position = Begin + value;
            _position = value;
        }
    }


    public SubStream(Stream stream, long begin, long length)
    {
        BaseStream = stream;
        Begin = begin;
        End = begin + length;
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void EnsurePosition()
    {
        if(BaseStream.Position - Begin != _position)
        {
            Position = _position;
        }
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
        _position += bytesRead;

        return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsurePosition();
        _position += count;
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

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Flush()
    {
        BaseStream.Flush();
    }
}