using API.Common;
using Application.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthenticationController : BaseApiController
    {
        public AuthenticationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IActionResult> Login(LoginRq rq)
        {
            return Ok(await this.RunAsync<LoginService, LoginRq, LoginRs>(rq));
        }

        public async Task<IActionResult> RefreshToken(RefreshTokenRq rq)
        {
            return Ok(await this.RunAsync<RefreshTokenService, RefreshTokenRq, RefreshTokenRs>(rq));
        }
    }
}