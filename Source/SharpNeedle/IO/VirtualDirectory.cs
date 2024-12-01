namespace SharpNeedle.IO;

public class VirtualDirectory : IDirectory
{
    public IDirectory Parent { get; protected set; }
    public string Path { get; protected set; }
    public string Name { get; set; }
    public Dictionary<string, VirtualFile> Files { get; set; } = [];
    public Dictionary<string, VirtualDirectory> Directories { get; set; } = [];

    public VirtualDirectory(string name) : this(name, null)
    {
        Path = name;
    }

    public VirtualDirectory(string name, VirtualDirectory parent)
    {
        Name = name;
        Parent = parent;

        if(parent != null)
        {
            Path = System.IO.Path.Combine(parent.Path, Name);
        }
    }

    public IFile this[string path]
    {
        get
        {
            string[] names = path.Split('/', '\\', StringSplitOptions.RemoveEmptyEntries);
            VirtualDirectory current = this;

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
                        if(isLast && current.Files.TryGetValue(name, out VirtualFile vFile))
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
    }

    public IFile CreateFile(string name)
    {
        VirtualFile file = new(name, this);
        Files.Add(name, file);
        return file;
    }

    public IDirectory CreateDirectory(string name)
    {
        VirtualDirectory dir = new(name, this);
        Directories.Add(name, dir);
        return dir;
    }

    public IFile Add(IFile file)
    {
        if(file is VirtualFile vFile)
        {
            vFile.Parent = this;
            vFile.Path = System.IO.Path.Combine(Path, vFile.Name);
            Files.Add(vFile.Name, vFile);
            return vFile;
        }

        vFile = new VirtualFile(file.Name, this);
        Files.Add(file.Name, vFile);
        vFile.BaseStream = file.Open(FileAccess.ReadWrite);
        return vFile;
    }

    public IDirectory OpenDirectory(string name)
    {
        return Directories.TryGetValue(name, out VirtualDirectory dir) ? dir : null;
    }

    public bool DeleteFile(string name)
    {
        return Files.Remove(name);
    }

    public bool DeleteDirectory(string name)
    {
        return Directories.Remove(name);
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

    public IEnumerable<IDirectory> GetDirectories()
    {
        return Directories.Values;
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