namespace SharpNeedle
{
    public static class Extensions
    {
        public static string GetIdentifier(this IResource res)
            => ResourceManager.GetIdentifier(res.GetType());

        public static bool IndexOf(this string str, char value, out int index)
        {
            index = str.IndexOf(value);
            return index >= 0;
        }
    }
}
