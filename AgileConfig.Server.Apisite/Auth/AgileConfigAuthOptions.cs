using Microsoft.AspNetCore.Authentication;

namespace AgileConfig.Server.Apisite.Auth
{
    public class AgileConfigAuthOptions : AuthenticationSchemeOptions
    {
        public string UserInfoEndpoint { get; set; }
    }
}
