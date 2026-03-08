namespace MealShareDotNet.Core.Utils;

public static class ConnectionStringUtil
{
    public static string GenerateConnectionString(string template)
    {
        return template
            .Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory)
            .Replace("{Sep}", Path.DirectorySeparatorChar.ToString());
    }
}
