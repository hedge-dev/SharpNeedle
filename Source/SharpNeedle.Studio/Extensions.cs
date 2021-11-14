namespace SharpNeedle.Studio;
using System.Reflection;

public static class Extensions
{
    public static IEnumerable<KeyValuePair<Type, TAttribute>> GetAttributedTypes<TAttribute>(this Assembly assembly) where TAttribute : Attribute
    {
        return from type in assembly.GetTypes()
            let attribute = type.GetCustomAttribute<TAttribute>()
            where attribute != null
            select new KeyValuePair<Type, TAttribute>(type, attribute);
    }

    public static TFunc GetAttributedFunction<TAttribute, TFunc>(this Type type) where TAttribute : Attribute where TFunc : Delegate
    {
        return (from m in type.GetMethods()
            where m.GetCustomAttribute<TAttribute>() != null
            select m.CreateDelegate<TFunc>()).FirstOrDefault();
    }
}