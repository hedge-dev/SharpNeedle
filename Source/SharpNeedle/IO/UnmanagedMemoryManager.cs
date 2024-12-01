namespace SharpNeedle.IO;

using System.Buffers;

public sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
{
    private T* _ptr;
    private int _length;


    public UnmanagedMemoryManager() { }

    public UnmanagedMemoryManager(ReadOnlySpan<T> span)
    {
        fixed(T* ptr = &MemoryMarshal.GetReference(span))
        {
            Swap(ptr, span.Length);
        }
    }


    public void Swap(T* ptr, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        _ptr = ptr;
        _length = length;
    }

    public override Span<T> GetSpan()
    {
        return new(_ptr, _length);
    }

    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if(elementIndex < 0 || elementIndex >= _length)
        {
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        }

        return new MemoryHandle(_ptr + elementIndex);
    }

    public override void Unpin() { }

    protected override void Dispose(bool disposing) { }
}