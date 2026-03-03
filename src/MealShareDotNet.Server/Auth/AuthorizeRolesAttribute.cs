using Microsoft.AspNetCore.Authorization;

namespace MealShareDotNet.Server.Auth;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(UserRoles roles)
    {
        Roles = roles.ToString().Replace(" ", String.Empty);
    }
}
