﻿namespace SharpNeedle.Utilities;

using System.Diagnostics.CodeAnalysis;

public static class StreamExtension
{
    public static T ReadStructure<T>(this Stream stream)
    {
        int structureSize = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[structureSize];

        if (stream.Read(buffer, 0, structureSize) != structureSize)
        {
            throw new EndOfStreamException("could not read all of data for structure");
        }

        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T))!;

        handle.Free();

        return structure;
    }

    public static T ReadStructure<T>(this Stream stream, int size)
    {
        int structureSize = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[Math.Max(structureSize, size)];

        if (stream.Read(buffer, 0, size) != size)
        {
            throw new EndOfStreamException("could not read all of data for structure");
        }

        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T))!;

        handle.Free();

        return structure;
    }

    public static T[] ReadStructures<T>(this Stream stream, int count)
    {
        int structureSize = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[structureSize * count];

        if (stream.Read(buffer, 0, structureSize * count) != structureSize * count)
        {
            throw new EndOfStreamException("could not read all of data for structures");
        }

        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        T[] structArray = new T[count];

        IntPtr bufferPtr = handle.AddrOfPinnedObject();

        for (int i = 0; i < count; i++)
        {
            structArray[i] = (T)Marshal.PtrToStructure(bufferPtr, typeof(T))!;
            bufferPtr += structureSize;
        }

        handle.Free();

        return structArray;
    }

    public static void WriteStructure<T>(this Stream stream, [DisallowNull] T structure)
    {
        int structureSize = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[structureSize];
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);

        handle.Free();

        stream.Write(buffer, 0, buffer.Length);
    }

    public static void WriteStructure<T>(this Stream stream, [DisallowNull] T structure, int size)
    {
        int structureSize = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[Math.Max(structureSize, size)];
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);

        handle.Free();

        stream.Write(buffer, 0, buffer.Length);
    }

    public static void WriteStructures<T>(this Stream stream, T[] structArray, int count)
    {
        int structureSize = Marshal.SizeOf(typeof(T));
        byte[] buffer = new byte[structureSize * count];
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr bufferPtr = handle.AddrOfPinnedObject();

        for (int i = 0; i < count; i++)
        {
            Marshal.StructureToPtr(structArray[i]!, bufferPtr, false);
            bufferPtr += structureSize;
        }

        handle.Free();

        stream.Write(buffer, 0, buffer.Length);
    }

    public static byte[] ReadBytes(this Stream stream, int count)
    {
        byte[] bytes = new byte[count];
        stream.Read(bytes, 0, count);
        return bytes;
    }

    public static byte[] ReadAllBytes(this Stream @this)
    {
        long positionSave = @this.Position;

        byte[] buffer = new byte[@this.Length];
        @this.Position = 0;
        @this.Read(buffer, 0, buffer.Length);
        @this.Position = positionSave;

        return buffer;
    }

    public static Span<byte> AsSpan(this MemoryStream stream)
    {
        return new(stream.GetBuffer(), 0, (int)stream.Length);
    }

    public static Span<byte> AsSpan(this MemoryStream stream, int start, int length)
    {
        return new(stream.GetBuffer(), start, length);
    }

    public static ReadOnlySpan<byte> AsReadOnlySpan(this MemoryStream stream)
    {
        return new(stream.GetBuffer(), 0, (int)stream.Length);
    }

    public static ReadOnlySpan<byte> AsReadOnlySpan(this MemoryStream stream, int start, int length)
    {
        return new(stream.GetBuffer(), start, length);
    }
}