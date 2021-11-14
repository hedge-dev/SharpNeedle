using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SharpNeedle.IO;

namespace SharpNeedle
{
    public class ResourceManager : IResourceManager
    {
        private static readonly Dictionary<Type, string> mTypes = new();
        private static readonly Dictionary<string, BinaryResourceAttribute> mResourceTypes = new();

        private readonly Dictionary<string, IResource> mResources = new();
        private readonly ConditionalWeakTable<IResource, string> mResourceTable = new();

        static ResourceManager()
        {
            RegisterResourceTypes(typeof(ResourceManager).Assembly);
        }

        private IResource OpenBase(IFile file, Type type, bool resolveDepends = true)
        {
            var path = file.Path;
            if (mResources.ContainsKey(path))
                return mResources[path];

            type ??= DetectType(file)?.Owner;

            if (type != null)
            {
                var res = (IResource)Activator.CreateInstance(type);
                res.Read(file);
                mResources.Add(path, res);
                mResourceTable.Add(res, path);
                if (resolveDepends)
                    res.ResolveDependencies(file.Parent);

                return res;
            }

            var rawResource = new ResourceRaw(file);
            mResources.Add(path, rawResource);
            mResourceTable.Add(rawResource, path);
            return rawResource;
        }

        public T Open<T>(IFile file, bool resolveDepends = true) where T : IResource, new()
        {
            return (T)OpenBase(file, typeof(T), resolveDepends);
        }

        public IResource Open(IFile file, bool resolveDepends = true)
        {
            return OpenBase(file, null, resolveDepends);
        }

        public bool IsOpen(string path)
            => mResources.ContainsKey(path);

        public void Close(IResource res)
        {
            if (!mResourceTable.TryGetValue(res, out var key))
                return;

            mResources.Remove(key);
            mResourceTable.Remove(res);
            res.Dispose();
        }

        public static void RegisterResourceTypes(Assembly assembly, bool ignoreRegistered = true)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetCustomAttribute<BinaryResourceAttribute>() != null))
            {
                var attribute = type.GetCustomAttribute<BinaryResourceAttribute>();
                attribute.Owner = type;

                if (mResourceTypes.ContainsKey(attribute.Id) && ignoreRegistered)
                    continue;

                foreach (var method in type.GetMethods())
                {
                    var checkAttribute = method.GetCustomAttribute<ResourceCheckFunctionAttribute>();
                    if (checkAttribute == null)
                        continue;

                    attribute.CheckResourceFunc = (Func<IFile, bool>)method.CreateDelegate(typeof(Func<IFile, bool>));
                    break;
                }
                mTypes.Add(type, attribute.Id);
                mResourceTypes.Add(attribute.Id, attribute);
            }
        }

        public static BinaryResourceAttribute DetectType(IFile file)
        {
            foreach (var value in mResourceTypes.Values)
            {
                if (value.CheckResourceFunc == null)
                    continue;

                if (value.CheckResourceFunc(file))
                    return value;
            }
            return null;
        }

        public static string GetIdentifier(Type type)
            => mTypes.TryGetValue(type, out string value) ? value : null;

        public static ResourceType? GetType(Type type)
        {
            if (mTypes.TryGetValue(type, out string value))
                return mResourceTypes[value].Type;

            return null;
        }

        public void Dispose()
        {
            foreach (var resource in mResources.Values)
                resource.Dispose();

            mResources.Clear();
            mResourceTable.Clear();
        }
    }
}
