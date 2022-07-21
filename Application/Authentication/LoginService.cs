using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Settings;
using Application.Jwt;
using Application.Users;

namespace Application.Authentication
{
    public class LoginRq : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRs : BaseResponse
    {
        public string JwtAccessToken { get; set; }
        public string JwtRefreshToken { get; set; }
        public DateTime JwtAccessTokenExpiresOn { get; set; }
        public User User { get; set; }
    }

    public class LoginService : BaseService<LoginRq, LoginRs>
    {
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
        private readonly IJwtManager _jwtManager;
        private readonly IRefreshTokenManager _refreshTokenService;

        public LoginService(IUserService userService, IJwtManager jwtManager, IRefreshTokenManager refreshTokenService, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _refreshTokenService = refreshTokenService;
            _jwtManager = jwtManager;
            _userService = userService;
        }

        protected override async Task<LoginRs> DoRunAsync(LoginRq rq)
        {
            User user = await _userService.FindByUsernameAsync(rq.Username);
            if (user == null)
            {
                throw new ServiceException("User not found");
            }

            bool passwordVerified = await _userService.CheckPasswordSigninAsync(rq.Username, rq.Password);
            if (!passwordVerified)
            {
                throw new ServiceException("Login error");
            }

            JwtInfo accessTokenInfo = new JwtInfo(user.UserName, JwtType.Access, _appSettings.JwtAccessTokenExpiryDuration);
            string accessToken = _jwtManager.GenerateToken(accessTokenInfo);

            JwtInfo refreshTokenInfo = new JwtInfo(user.UserName, JwtType.Refresh, _appSettings.JwtAccessTokenExpiryDuration);
            string refreshToken = _jwtManager.GenerateToken(refreshTokenInfo);

            await _refreshTokenService.SaveTokenAsync(user.UserName, refreshToken);

            return new LoginRs
            {
                User = user,
                JwtAccessToken = accessToken,
                JwtAccessTokenExpiresOn = accessTokenInfo.Expires,
                JwtRefreshToken = refreshToken
            };
        }
    }
}