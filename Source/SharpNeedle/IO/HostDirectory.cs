using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SharpNeedle.IO
{
    public class HostDirectory : IDirectory
    {
        public string FullPath { get; set; }

        public IDirectory Parent
            => FromPath(Path.GetDirectoryName(Path.IsPathRooted(FullPath.AsSpan())
                ? FullPath
                : Path.TrimEndingDirectorySeparator(FullPath)));

        public string Name
        {
            get
            {
                var name = Path.GetFileName(FullPath);
                if (string.IsNullOrEmpty(name))
                    return FullPath;

                return name;
            }

            set => throw new NotSupportedException();
        }

        public IFile this[string name] => GetFile(name);
        
        public bool DeleteFile(string name)
        {
            var path = Path.Combine(FullPath, name);
            if (!File.Exists(path))
                return false;

            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDirectory(string name)
        {
            var path = Path.Combine(FullPath, name);
            if (!Directory.Exists(path))
                return false;

            try
            {
                Directory.Delete(path, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static HostDirectory FromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (!Directory.Exists(path))
                return null;

            return new HostDirectory
            {
                FullPath = path
            };
        }

        private HostDirectory()
        {

        }

        public HostDirectory(string path)
        {
            FullPath = Path.GetFullPath(path);
            if (!Directory.Exists(FullPath))
                throw new DirectoryNotFoundException(path);
        }

        public IFile GetFile(string name)
        {
            var path = Path.Combine(FullPath, name);
            
            return File.Exists(path) ? new HostFile(path) : null;
        }

        public IFile Create(string name)
            => HostFile.Create(Path.Combine(FullPath, name));

        public IFile Add(IFile file)
        {
            var destFile = Create(file.Name);
            using var destStream = destFile.Open(FileAccess.Write);
            using var srcStream = file.Open();
            srcStream.CopyTo(destStream);

            return destFile;
        }

        public IEnumerable<IDirectory> GetDirectories()
        {
            foreach (var file in Directory.EnumerateDirectories(FullPath))
            {
                yield return new HostDirectory(file);
            }
        }

        public IEnumerator<IFile> GetEnumerator()
        {
            foreach (var file in Directory.EnumerateFiles(FullPath))
            {
                yield return new HostFile(file);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public override string ToString()
            => Name;
    }
}
