namespace SharpNeedle.IO;

using System.IO;

public class VirtualDirectory : IDirectory
{
    public IDirectory? Parent { get; protected set; }

    public string Path { get; protected set; }

    public string Name { get; set; }

    public Dictionary<string, VirtualFile> Files { get; set; } = [];

    public Dictionary<string, VirtualDirectory> Directories { get; set; } = [];

    public IFile? this[string path] => GetFile(path);



    public VirtualDirectory(string name, VirtualDirectory? parent)
    {
        Name = name;
        Parent = parent;

        if(parent != null)
        {
            Path = System.IO.Path.Combine(parent.Path, Name);
        }
        else
        {
            Path = Name;
        }
    }

    public VirtualDirectory(string name) : this(name, null) { }


    public IEnumerable<IDirectory> GetDirectories()
    {
        return Directories.Values;
    }

    public IDirectory? GetDirectory(string name)
    {
        Directories.TryGetValue(name, out VirtualDirectory? dir);
        return dir;
    }

    public IDirectory CreateDirectory(string name)
    {
        if(Directories.ContainsKey(name))
        {
            throw new InvalidOperationException($"Failed to create directory \"{name}\"! Virtual directory \"{Path}\" already has a directory with the same name!");
        }

        VirtualDirectory dir = new(name, this);
        Directories.Add(name, dir);
        return dir;
    }

    public bool DeleteDirectory(string name)
    {
        return Directories.Remove(name);
    }


    public IFile? GetFile(string path)
    {
        string[] names = path.Split('/', '\\', StringSplitOptions.RemoveEmptyEntries);
        VirtualDirectory? current = this;

        for(int i = 0; i < names.Length; i++)
        {
            if(current == null)
            {
                return null;
            }

            string name = names[i];
            bool isLast = i == names.Length - 1;

            switch(name)
            {
                case ".":
                    continue;

                case "..":
                    current = current.Parent as VirtualDirectory;
                    continue;

                default:
                {
                    if(isLast && current.Files.TryGetValue(name, out VirtualFile? vFile))
                    {
                        return vFile;
                    }

                    current.Directories.TryGetValue(name, out current);
                    continue;
                }
            }
        }

        return null;
    }

    public IFile CreateFile(string name, bool overwrite = true)
    {
        if(Files.ContainsKey(name) && !overwrite)
        {
            throw new InvalidOperationException($"Creating of file \"{name}\" failed! Virtual directory \"{Path}\" already has a file with the same name!");
        }

        VirtualFile file = new(name, this);
        Files[name] = file;
        return file;
    }

    public IFile AddFile(IFile file, bool overwrite = true)
    {
        if(Files.ContainsKey(file.Name) && !overwrite)
        {
            throw new InvalidOperationException($"Adding of file \"{file.Name}\" failed! Virtual directory \"{Path}\" already has a file with the same name!");
        }

        if(file is VirtualFile vFile)
        {
            vFile.Parent = this;
            vFile.Path = System.IO.Path.Combine(Path, vFile.Name);
        }
        else
        {
            vFile = new(file.Name, this)
            {
                BaseStream = file.Open(FileAccess.ReadWrite)
            };
        }

        Files[vFile.Name] = vFile;
        return vFile;
    }

    public bool DeleteFile(string name)
    {
        return Files.Remove(name);
    }


    public override string ToString()
    {
        return Name;
    }

    internal void FileNameChanged(VirtualFile file, string oldName, string newName)
    {
        Files.Remove(oldName);
        Files.Add(newName, file);
    }


    public IEnumerator<IFile> GetEnumerator()
    {
        return Files.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}