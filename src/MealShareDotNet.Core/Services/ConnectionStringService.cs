namespace MealShareDotNet.Core.Services;

public static class ConnectionStringService
{
    public static string GenerateConnectionString(string template)
    {
        return template
            .Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory)
            .Replace("{Sep}", Path.DirectorySeparatorChar.ToString());
    }
}
