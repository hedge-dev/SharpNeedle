namespace SharpNeedle.Utilities;

public static class MemoryExtensions
{
    /// <summary>
    /// This method is mostly unsafe, use with caution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="span"></param>
    /// <returns></returns>
    public static Memory<T> AsMemory<T>(this Span<T> span) where T : unmanaged
    {
        UnmanagedMemoryManager<T> manager = new(span);
        return manager.Memory;
    }
}