using Microsoft.AspNetCore.Authentication;

namespace MealShareDotNet.Server.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";

    public string ApiKeyHeaderName { get; init; } = "X-API-KEY";

    public IList<ApiKey> ApiKeys { get; set; } = [];
}
