using System.Linq;

public static class ExtensionMethods
{
    public static string RemoveDigits(this string str)
    {
        return new string(str.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
    }
}
