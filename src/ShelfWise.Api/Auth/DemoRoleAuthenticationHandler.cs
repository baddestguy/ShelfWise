using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ShelfWise.Api.Auth
{
    public class DemoRoleAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "DemoRole";
        public const string RoleHeaderName = "X-User-Role";

        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin",
            "Librarian",
            "Patron"
        };

        public DemoRoleAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(RoleHeaderName, out var values))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var role = values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(role) || !AllowedRoles.Contains(role))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid demo role."));
            }

            var normalizedRole = AllowedRoles.First(allowed => string.Equals(allowed, role, StringComparison.OrdinalIgnoreCase));
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, $"Demo {normalizedRole}"),
                new Claim(ClaimTypes.Role, normalizedRole)
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
