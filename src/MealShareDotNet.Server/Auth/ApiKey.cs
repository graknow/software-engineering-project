namespace MealShareDotNet.Server.Auth;

public class ApiKeyConfig
{
    public string Name { get; init; } = String.Empty;
    public string Value { get; init; } = String.Empty;
    public string Roles { get; init; } = String.Empty;

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

public class ApiKey
{
    public string Name { get; init; } = String.Empty;
    public string Value { get; init; } = String.Empty;
    public UserRoles Roles { get; init; }
}
