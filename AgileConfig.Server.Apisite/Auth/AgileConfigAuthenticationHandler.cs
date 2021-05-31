using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AgileConfig.Server.Apisite.Auth
{
    public class AgileConfigAuthenticationHandler : AuthenticationHandler<AgileConfigAuthOptions>
    {
        public AgileConfigAuthenticationHandler(
            IOptionsMonitor<AgileConfigAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.NoResult();
            if (!authorizationHeader.StartsWith(AgileConfigAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unauthorized");

            string token = authorizationHeader.Substring(AgileConfigAuthenticationDefaults.AuthenticationScheme.Length).Trim();
            if (string.IsNullOrEmpty(token))
                return AuthenticateResult.Fail("Unauthorized");

            try
            {
                return await ValidateTokenAsync(token);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private async Task<AuthenticateResult> ValidateTokenAsync(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting.Instance.SecurityKey));
            var myIssuer = JwtSetting.Instance.Issuer;
            var myAudience = JwtSetting.Instance.Audience;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);

                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }
    }
}
