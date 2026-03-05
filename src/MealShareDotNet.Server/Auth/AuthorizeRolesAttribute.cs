using Microsoft.AspNetCore.Authorization;

namespace MealShareDotNet.Server.Auth;

/// <summary>
/// Configures authorization roles based on the custom UserRoles enum.
/// </summary>
public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(UserRoles roles)
    {
        Roles = roles.ToString().Replace(" ", String.Empty);
    }
}
