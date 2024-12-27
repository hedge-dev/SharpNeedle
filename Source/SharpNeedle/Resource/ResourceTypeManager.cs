namespace SharpNeedle.Resource;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ResourceTypeManager
{
    private static readonly Dictionary<Type, string> _types = [];
    private static readonly Dictionary<string, NeedleResourceAttribute> _resourceTypes = [];


    static ResourceTypeManager()
    {
        RegisterResourceTypes(typeof(ResourceManager).Assembly);
    }


    public static void RegisterResourceTypes(Assembly assembly, bool ignoreRegistered = true)
    {
        foreach(Type type in assembly.GetTypes().Where(t => t.GetCustomAttribute<NeedleResourceAttribute>() != null))
        {
            NeedleResourceAttribute attribute = type.GetCustomAttribute<NeedleResourceAttribute>()!;
            attribute.Owner = type;

            if(_resourceTypes.ContainsKey(attribute.Id) && ignoreRegistered)
            {
                continue;
            }

            foreach(MethodInfo method in type.GetMethods())
            {
                ResourceCheckFunctionAttribute? checkAttribute = method.GetCustomAttribute<ResourceCheckFunctionAttribute>();
                if(checkAttribute == null)
                {
                    continue;
                }

                attribute.CheckResourceFunc = (Func<IFile, bool>)method.CreateDelegate(typeof(Func<IFile, bool>));
                break;
            }

            _types.Add(type, attribute.Id);
            _resourceTypes.Add(attribute.Id, attribute);
        }
    }

    public static NeedleResourceAttribute? DetectType(IFile file)
    {
        foreach(NeedleResourceAttribute value in _resourceTypes.Values)
        {
            if(value.CheckResource(file))
            {
                return value;
            }
        }

        return null;
    }

    public static string? GetIdentifier(Type type)
    {
        _types.TryGetValue(type, out string? value);
        return value;
    }

    public static ResourceType? GetType(Type type)
    {
        if(_types.TryGetValue(type, out string? value))
        {
            return _resourceTypes[value].Type;
        }

        return null;
    }

}
