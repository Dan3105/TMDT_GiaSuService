using GiaSuService.Configs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace GiaSuService.Handler
{
    public class MyAuthenticationHandler : AuthenticationHandler<MyCustomAuthentiactionOptions>
    {
        public MyAuthenticationHandler(
        IOptionsMonitor<MyCustomAuthentiactionOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var user = Context.User;
            if(user != null)
            {
                AuthenticationTicket ticket = new AuthenticationTicket(user, AppConfig.AUTHSCHEME);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
        }
    }

    public class MyCustomAuthentiactionOptions : AuthenticationSchemeOptions
    {

    }
}
