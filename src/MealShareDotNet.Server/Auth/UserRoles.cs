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

/// <summary>
/// Extension methods for the UserRoles enum.
/// </summary>
public static class UserRolesExtensions
{
    /// <summary>
    /// Converts the enum value to a valid authentication role string.
    /// </summary>
    /// <returns>
    /// A valid role string.
    /// </returns>
    public static string ToRoleString(this UserRoles roles)
    {
        return roles.ToString().Replace(" ", String.Empty);
    }

    /// <summary>
    /// Converts the enum value to an array of role strings.
    /// </summary>
    /// <returns>
    /// An array of valid role strings.
    /// </returns>
    public static string[] ToArray(this UserRoles roles)
    {
        return roles.ToRoleString().Split(',');
    }

    /// <summary>
    /// Attempts to parse a given role string into a UserRoles enum.
    /// </summary>
    /// <returns>
    /// True: The parsing was successful.  False: The parsing failed.
    /// </returns>
    public static bool TryParse(string roleString, out UserRoles roles)
    {
        roles = 0;

        foreach (var value in roleString.Split(','))
        {
            if (Enum.TryParse(typeof(UserRoles), value.Trim(), out var role))
            {
                roles |= (UserRoles)role;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
