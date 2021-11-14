using System.Collections.Generic;

namespace SharpNeedle.IO
{
    public interface IDirectory : IEnumerable<IFile>
    {
        IDirectory Parent { get; }

        string Name { get; set; }

        IFile this[string name] { get; }

        IEnumerable<IDirectory> GetDirectories();
    }
}
