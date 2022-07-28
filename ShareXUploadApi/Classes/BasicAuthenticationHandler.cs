using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Ubiety.Dns.Core;

namespace ShareXUploadApi.Classes
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IDBService _DBService;

        public BasicAuthenticationHandler(IDBService dbService, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
            _DBService = dbService;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                string? token = authHeader["Basic ".Length..].Trim();

                string? credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                string[]? credentials = credentialstring.Split(':');

                bool isAuthenticated = await _DBService.IsUserAuthenticated(new() { Username = credentials[0], Password = credentials[1] });

                if (isAuthenticated)
                {
                    Claim[]? claims = new[] { new Claim("name", credentials[0]), new Claim(ClaimTypes.Role, "User") };
                    ClaimsIdentity? identity = new(claims, "Basic");
                    ClaimsPrincipal? claimsPrincipal = new(identity);
                    return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
                }
                
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
            else
            {                 
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}
