using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Settings;
using Application.Jwt;
using Application.Users;

namespace Application.Authentication
{
    public class RefreshTokenRq : BaseRequest
    {
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRs : BaseResponse
    {
        public string JwtRefreshToken { get; set; }
        public string JwtAccessToken { get; set; }
        public DateTime JwtAccessTokenExpiresOn { get; set; }
        public DateTime? JwtRefreshTokenExpiresOn { get; set; }
    }

    public class RefreshTokenService : BaseService<RefreshTokenRq, RefreshTokenRs>
    {
        private readonly IJwtManager _jwtManager;
        private readonly IUserService _userService;
        private readonly IRefreshTokenManager _refreshTokenManager;
        private readonly AppSettings _appSettings;
        public RefreshTokenService(IJwtManager jwtManager, IUserService userService, IRefreshTokenManager refreshTokenManager, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _refreshTokenManager = refreshTokenManager;
            _userService = userService;
            _jwtManager = jwtManager;
        }
        protected override async Task<RefreshTokenRs> DoRunAsync(RefreshTokenRq rq)
        {
            JwtInfo refreshTokenInfo = _jwtManager.Validate(rq.RefreshToken, JwtType.Refresh);
            User user = await _userService.FindByUsernameAsync(refreshTokenInfo.UserName);
            if (user == null)
            {
                throw new ServiceException("User not found");
            }   

            JwtInfo newRefreshTokenInfo = new JwtInfo(user.UserName, JwtType.Refresh, _appSettings.JwtRefreshTokenExpiryDuration);
            string newRefreshToken = await _refreshTokenManager.ValidateAndRefresh(user.UserName, rq.RefreshToken, newRefreshTokenInfo);

            JwtInfo newAccessTokenInfo = new JwtInfo(user.UserName, JwtType.Access, _appSettings.JwtAccessTokenExpiryDuration);
            string accessToken = _jwtManager.GenerateToken(newAccessTokenInfo);

            RefreshTokenRs rs = new()
            {
                JwtAccessToken = accessToken,
                JwtAccessTokenExpiresOn = newAccessTokenInfo.Expires,
                JwtRefreshToken = newRefreshToken,
                JwtRefreshTokenExpiresOn = newRefreshTokenInfo.Expires
            };

            return rs;
        }
    }
}