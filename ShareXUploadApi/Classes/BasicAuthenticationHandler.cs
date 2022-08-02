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
            string? username = Request.Headers["username"].ToString();
            string? password = Request.Headers["password"].ToString();
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {                
                bool isAuthenticated = await _DBService.IsUserAuthenticated(new() { Username = username, Password = password });

                if (isAuthenticated)
                {
                    Claim[]? claims = new[] { new Claim("name", username), new Claim(ClaimTypes.Role, "User") };
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
