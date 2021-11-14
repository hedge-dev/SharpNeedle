using System.Collections.Generic;
using SharpNeedle.IO;

namespace SharpNeedle
{
    [BinaryResource("internal/raw", ResourceType.Raw)]
    public class ResourceRaw : IResource
    {
        public IFile File { get; }
        public string Name { get; set; }

        public ResourceRaw(IFile file)
        {
            File = file;
            Name = file.Name;
        }

        public void Read(IFile file)
        {

        }

        public void Write(IFile file)
        {

        }

        public IReadOnlyList<ResourceDependency> GetDependencies()
        {
            return new List<ResourceDependency>();
        }

        public void ResolveDependencies(IDirectory dir)
        {

        }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}
