using SharpNeedle.IO;

namespace SharpNeedle
{
    public static class ResourceExtensions
    {
        public static T Open<T>(this IResourceManager manager, string path, bool resolveDepends = true) where T : IResource, new()
        {
            var file = FileSystem.Open(path);
            return manager.Open<T>(file);
        }

        public static IResource Open(this IResourceManager manager, string path, bool resolveDepends = true)
        {
            var file = FileSystem.Open(path);
            return manager.Open(file, resolveDepends);
        }

        public static void Read(this IResource resource, string path, bool resolveDepends = true)
        {
            var file = FileSystem.Open(path);
            resource.Read(file);

            if (resolveDepends)
                resource.ResolveDependencies(file.Parent);
        }
    }
}
