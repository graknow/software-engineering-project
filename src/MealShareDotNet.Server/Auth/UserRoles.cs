namespace MealShareDotNet.Server.Auth;

/// <summary>
/// Valid user roles.
/// </summary>
[Flags]
public enum UserRoles
{
    USER = 1,
    MODERATOR = 1 << 1,
    OWNER = 1 << 2,
}
