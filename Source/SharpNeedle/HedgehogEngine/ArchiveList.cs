using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amicitia.IO.Binary;
using Amicitia.IO.Streams;
using SharpNeedle.IO;
using SharpNeedle.Utilities;

namespace SharpNeedle.HedgehogEngine
{
    [BinaryResource(ResourceType)]
    public class ArchiveList : ResourceBase, IDirectory, IStreamable
    {
        public const string ResourceType = "hh/archive-list";
        public static uint Signature { get; } = BinaryHelper.MakeSignature<uint>("ARL2");

        public IDirectory Parent { get; private set; }
        public bool DependsResolved { get; private set; }
        public List<Archive> Archives { get; private set; }
        public List<uint> ArchiveSizes { get; set; } = new();
        public HashSet<string> Files { get; set; } = new();
        public IFile this[string name] => GetFile(name);

        public IEnumerable<IDirectory> GetDirectories() => Enumerable.Empty<IDirectory>();

        public override void Read(IFile file)
        {
            Name = file.Name;
            Parent = file.Parent;
            using var stream = file.Open();
            using var reader = new BinaryValueReader(stream, StreamOwnership.Retain, Endianness.Little);

            var sig = reader.ReadUInt32();
            if (sig != Signature)
                throw new Exception("Invalid ARL file");
            
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ArchiveSizes.Add(reader.ReadUInt32());
            }

            while (stream.Position < stream.Length)
            {
                var name = reader.ReadString(StringBinaryFormat.PrefixedLength8);
                
                if (!Files.Contains(name))
                    Files.Add(name);
            }
        }

        public override IReadOnlyList<ResourceDependency> GetDependencies()
        {
            var result = new List<ResourceDependency>();
            var baseName = Path.GetFileNameWithoutExtension(Name);

            for (int i = 0; i < ArchiveSizes.Count; i++)
            {
                result.Add(new ResourceDependency()
                {
                    Name = $"{baseName}.ar.{i:00}",
                    Id = Archive.ResourceType
                });
            }

            return result;
        }

        public override void Write(IFile file)
        {
            throw new NotImplementedException();
        }

        public override void ResolveDependencies(IDirectory dir)
        {
            if (ArchiveSizes.Count > 0)
                Archives = new List<Archive>(ArchiveSizes.Count);

            var baseName = Path.GetFileNameWithoutExtension(Name);
            for (int i = 0; i < ArchiveSizes.Count; i++)
            {
                var name = $"{baseName}.ar.{i:00}";
                var file = dir[name];
                if (file == null)
                    throw new FileNotFoundException(name);

                var archive = new Archive { Name = name };
                archive.Read(file);
                Archives.Add(archive);
            }

            Parent = dir;
            DependsResolved = true;
        }

        [ResourceCheckFunction]
        public static bool IsResourceValid(IFile file)
        {
            using var stream = file.Open();
            return BitConverter.ToUInt32(stream.ReadBytes(sizeof(uint)), 0) == Signature;
        }

        public IFile GetFile(string name)
        {
            foreach (var archive in Archives)
            {
                var file = archive.GetFile(name);
                if (file != null)
                    return file;
            }

            return null;
        }

        public void LoadToMemory()
        {
            foreach (var archive in Archives)
                archive.LoadToMemory();
        }

        public override void Dispose()
        {
            if (Archives != null)
            {
                foreach (var archive in Archives)
                    archive.Dispose();
            }

            base.Dispose();
        }

        public IEnumerator<IFile> GetEnumerator()
        {
            if (Archives == null)
                yield break;

            foreach (var archive in Archives)
            {
                foreach (var file in archive)
                {
                    yield return file;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
