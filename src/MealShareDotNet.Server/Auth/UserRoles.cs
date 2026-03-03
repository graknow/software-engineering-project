namespace MealShareDotNet.Server.Auth;

[Flags]
public enum UserRoles
{
    USER = 1,
    MODERATOR = 1 << 1,
    OWNER = 1 << 2,
}
