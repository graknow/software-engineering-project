namespace MealShareDotNet.Server.Auth;

/// <summary>
/// Maps the appconfig api key settings directly to an object.
/// </summary>
public class ApiKeyConfig
{
    public string Name { get; init; } = String.Empty;
    public string Value { get; init; } = String.Empty;
    public string Roles { get; init; } = String.Empty;

    /// <summary>
    /// <para>Parses the configuration settings into an ApiKey object, which contains</para>
    /// <para>strongly typed fields rather than all strings.</para>
    /// </summary>
    /// <returns>
    /// An ApiKey object generated from the passed configuration settings.
    /// </returns>
    public ApiKey ToApiKey()
    {
        UserRoles roles = 0;

        foreach (var value in this.Roles.Split(','))
        {
            if (Enum.TryParse(typeof(UserRoles), value.Trim(), out var role))
            {
                roles |= (UserRoles)role;
            }
        }

        return new()
        {
            Name = this.Name,
            Value = this.Value,
            Roles = roles,
        };
    }
}

/// <summary>
/// ApiKey data used for authenticating/authorizing requests to the server.
/// </summary>
public class ApiKey
{
    public string Name { get; init; } = String.Empty;
    public string Value { get; init; } = String.Empty;
    public UserRoles Roles { get; init; }
}
