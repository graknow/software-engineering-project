namespace MealShareDotNet.Server.Auth;

/// <summary>
/// Maps the appconfig API key settings directly to an object.
/// </summary>
public class ApiKeyConfig
{
    public string Name { get; init; } = String.Empty;
    public string Value { get; init; } = String.Empty;
    public string Roles { get; init; } = String.Empty;
}

/// <summary>
/// API key data used for authenticating/authorizing requests to the server.
/// </summary>
public class ApiKey
{
    public string Name { get; init; } = String.Empty;
    public string Value { get; init; } = String.Empty;
    public UserRoles Roles { get; init; }

    /// <summary>
    /// <para>Attempts to parse the configuration settings into an ApiKey object,</para>
    /// <para> which contains strongly typed fields rather than all strings.</para>
    /// </summary>
    /// <returns>
    /// True: The parsing was successfull.  False: The parsing failed.
    /// </returns>
    public static bool TryParse(ApiKeyConfig config, out ApiKey key)
    {
        key = default!;

        if (UserRolesExtensions.TryParse(config.Roles, out var roles))
        {
            key = new()
            {
                Name = config.Name,
                Value = config.Value,
                Roles = roles
            };

            return true;
        }

        return false;
    }
}
