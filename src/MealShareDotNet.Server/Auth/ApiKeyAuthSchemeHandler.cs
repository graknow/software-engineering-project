using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MealShareDotNet.Server.Auth;

/// <summary>
/// An HTTPS request authentication scheme utilizing a custom ApiKey system.
/// </summary>
public class ApiKeyAuthSchemeHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
{
    public ApiKeyAuthSchemeHandler(
            IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(Options.ApiKeyHeaderName))
        {
            return AuthenticateResult.Fail("API key header missing.");
        }

        var requestedKey = Request.Headers[Options.ApiKeyHeaderName];
        var key = Options.ApiKeys.FirstOrDefault(k => k.Value.Equals(requestedKey));

        // Unauthorized key if result is null.
        if (key is null)
        {
            return AuthenticateResult.Fail("Provided key doesn't match any existing API keys.");
        }

        var claims = new List<Claim>();

        if (key.Roles != 0)
        {
            var roles = key.Roles.ToString().Replace(" ", String.Empty).Split(',');

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var claimsIdentity = new ClaimsIdentity(claims, this.Scheme.Name);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(claimsPrincipal, this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
