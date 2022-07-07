using API.Common;
using Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthenticationController : BaseApiController
    {
        public AuthenticationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRq rq)
        {
            return Ok(await this.RunAsync<LoginService, LoginRq, LoginRs>(rq));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            RefreshTokenRq rq = new()
            {
                RefreshToken = Request.Cookies["JwtRefreshToken"]
            };

            return Ok(await this.RunAsync<RefreshTokenService, RefreshTokenRq, RefreshTokenRs>(rq));
        }
    }
}