namespace SharpNeedle;
using System.Buffers;

public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
{
    private T* mPtr;
    private int mLength;

    public UnmanagedMemoryManager()
    {

    }

    public UnmanagedMemoryManager(ReadOnlySpan<T> span)
    {
        fixed(T* ptr = &MemoryMarshal.GetReference(span))
        {
            Swap(ptr, span.Length);
        }
    }

    public void Swap(T* ptr, int length)
    {
        if(length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        mPtr = ptr;
        mLength = length;
    }

    public override Span<T> GetSpan()
    {
        return new(mPtr, mLength);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if(elementIndex < 0 || elementIndex >= mLength)
        {
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        }

        return new MemoryHandle(mPtr + elementIndex);
    }

    public override void Unpin()
    {

    }

    protected override void Dispose(bool disposing)
    {

    }
}