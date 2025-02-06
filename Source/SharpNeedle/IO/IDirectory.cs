namespace SharpNeedle.IO;

/// <summary>
/// File directory interface.
/// </summary>
public interface IDirectory : IEnumerable<IFile>
{
    /// <summary>
    /// Containing directory of this directory, if it exists.
    /// </summary>
    IDirectory? Parent { get; }

    /// <summary>
    /// Full path to the directory (including name).
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Name of the directory.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Index accessor to <see cref="GetFile(string)"/>
    /// </summary>
    /// <param name="name">Name of the file to get.</param>
    IFile? this[string name] { get; }


    /// <summary>
    /// Returns an enumerator for all directories in this directory.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IDirectory> GetDirectories();

    /// <summary>
    /// Returns a directory by name. Returns <see langword="null"/> if it doesn't exist. 
    /// </summary>
    /// <param name="name">Name of the directory to get.</param>
    IDirectory? GetDirectory(string name);

    /// <summary>
    /// Creates a new directory. 
    /// <br/> Throws an exception if a directory with the same name already exists.
    /// </summary>
    /// <param name="name">Name of the directory to create.</param>
    /// <returns>The created directory.</returns>
    IDirectory CreateDirectory(string name);

    bool DeleteDirectory(string name);


    /// <summary>
    /// Attempts to get a file. Returns <see langword="null"/> if it doesn't exist.
    /// </summary>
    /// <param name="name">Name of the file to get.</param>
    IFile? GetFile(string name);

    /// <summary>
    /// Creates a new, empty file in this directory.
    /// <br/> Throws an exception if a file with the same name already exists and <paramref name="overwrite"/> is <see langword="false"/>.
    /// </summary>
    /// <param name="name">Name of the file to create.</param>
    /// <returns>The created file.</returns>
    IFile CreateFile(string name, bool overwrite = true);

    /// <summary>
    /// Add a file to this directory.
    /// <br/> Throws an exception if the file already exists and <paramref name="overwrite"/> is <see langword="false"/>.
    /// </summary>
    /// <param name="file">The file to copy.</param>
    /// <returns>The created copy.</returns>
    IFile AddFile(IFile file, bool overwrite = true);

    /// <summary>
    /// Deletes a file. Returns <see langword="true"/> when the file was successfully deleted, otherwise false.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool DeleteFile(string name);

}