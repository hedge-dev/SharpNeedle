namespace SharpNeedle;

public static class Extensions
{
    public static string GetIdentifier(this IResource res)
        => ResourceManager.GetIdentifier(res.GetType());

    public static bool IndexOf(this string str, char value, out int index)
    {
        index = str.IndexOf(value);
        return index >= 0;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(value))
            {
                return i;
            }
        }
        
        return -1;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, Predicate<T> predict)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (predict(list[i]))
            {
                return i;
            }
        }

        return -1;
    }
}