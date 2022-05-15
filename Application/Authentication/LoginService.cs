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
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LoginService : BaseService<LoginRq, LoginRs>
    {
        private const string CONFIG_NAME_JWT_ACCESS_EXPIRES_DURATION = "JwtAccessExpireDuration";
        private readonly AppSettings _appSettings;

        private readonly IUserService _userService;
        private readonly IJwtManager _jwtManager;
        private readonly IRefreshTokenManager _refreshTokenService;
        public LoginService(IUserService userService, IJwtManager jwtManager, IRefreshTokenManager refreshTokenService, AppSettings appSettings)
        {
            this._appSettings = appSettings;
            this._refreshTokenService = refreshTokenService;
            this._jwtManager = jwtManager;
            this._userService = userService;
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

            string accessToken = _jwtManager.GenerateToken(new JwtInfo(user.UserName, JwtType.Access, _appSettings.JwtAccessTokenExpiryDuration));
            string refreshToken = _jwtManager.GenerateToken(new JwtInfo(user.UserName, JwtType.Refresh, _appSettings.JwtAccessTokenExpiryDuration));

            await _refreshTokenService.SaveTokenAsync(user.UserName, refreshToken);

            return new LoginRs
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}