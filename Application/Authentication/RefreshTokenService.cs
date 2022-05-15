using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
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
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }

    public class RefreshTokenService : BaseService<RefreshTokenRq, RefreshTokenRs>
    {
        private readonly IJwtManager _jwtManager;
        private readonly IUserService _userService;
        private readonly IRefreshTokenManager _refreshTokenManager;
        public RefreshTokenService(IJwtManager jwtManager, IUserService userService, IRefreshTokenManager refreshTokenManager)
        {
            _refreshTokenManager = refreshTokenManager;
            _userService = userService;
            _jwtManager = jwtManager;
        }
        protected override async Task<RefreshTokenRs> DoRunAsync(RefreshTokenRq rq)
        {
            JwtInfo jwtInfo = _jwtManager.Validate(rq.RefreshToken, JwtType.Refresh);
            User user = await _userService.FindByUsernameAsync(jwtInfo.UserName);
            if (user == null)
            {
                throw new ServiceException("User not found");
            }

            string newRefreshToken = await _refreshTokenManager.ValidateAndRefresh(user.UserName, rq.RefreshToken);

            string accessToken = _jwtManager.GenerateToken(new JwtInfo(user.UserName, JwtType.Access, new TimeSpan(7, 0, 0)));

            RefreshTokenRs rs = new()
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };

            return rs;
        }
    }
}