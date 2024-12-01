namespace SharpNeedle.IO;

/// <summary>
/// File interface.
/// </summary>
public interface IFile : IDisposable
{
    /// <summary>
    /// Directory containing this file.
    /// </summary>
    IDirectory Parent { get; }

    /// <summary>
    /// Full path to the file (including the name).
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Name of the file.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Size of the file in bytes.
    /// </summary>
    long Length { get; }

    /// <summary>
    /// When the file was last modified.
    /// </summary>
    DateTime LastModified { get; set; }


    /// <summary>
    /// Opens a stream to access the file data. 
    /// <br/> May have already been opened.
    /// </summary>
    /// <param name="access">How to access the stream.</param>
    Stream Open(FileAccess access = FileAccess.Read);
}