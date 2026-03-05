using Microsoft.AspNetCore.Authentication;

namespace MealShareDotNet.Server.Auth;

/// <summary>
/// An options object for configuring the ApiKeyAuthScheme.
/// </summary>
public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";

    /// <summary>
    /// The HTTPS header to search for an ApiKey.
    /// </summary>
    public string ApiKeyHeaderName { get; init; } = "X-API-KEY";

    /// <summary>
    /// List of accepted ApiKey configurations.
    /// </summary>
    public IList<ApiKey> ApiKeys { get; set; } = [];
}
