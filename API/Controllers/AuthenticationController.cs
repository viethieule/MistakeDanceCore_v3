using API.Common;
using Application.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuthenticationController : BaseApiController
    {
        private const string COOKIE_JWT_REFRESH_TOKEN = "JwtRefreshToken";
        public AuthenticationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRq rq)
        {
            LoginRs rs = await this.RunAsync<LoginService, LoginRq, LoginRs>(rq);
            SetRefreshTokenCookie(rs.JwtRefreshToken);
            rs.JwtRefreshToken = null;
            return Ok(rs);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                RefreshTokenRq rq = new()
                {
                    RefreshToken = Request.Cookies[COOKIE_JWT_REFRESH_TOKEN]
                };

                if (string.IsNullOrEmpty(rq.RefreshToken))
                {
                    return Unauthorized();
                }

                RefreshTokenRs rs  = await this.RunAsync<RefreshTokenService, RefreshTokenRq, RefreshTokenRs>(rq);
                SetRefreshTokenCookie(rs.JwtRefreshToken);
                rs.JwtRefreshToken = string.Empty;
                return Ok(rs);
            }
            catch (Exception)
            {
                // log
                return Unauthorized();
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = DateTime.Now.AddDays(1)
                // Path = "/api/Authentication/RefreshTokens"
            };

            Response.Cookies.Append(COOKIE_JWT_REFRESH_TOKEN, refreshToken, cookieOptions);
        }
    }
}