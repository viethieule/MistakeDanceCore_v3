using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
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
        private readonly IUserService _userService;
        private readonly IJwtManager _jwtManager;
        public LoginService(IUserService userService, IJwtManager jwtManager)
        {
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

            string accessToken = _jwtManager.GenerateToken(new JwtInfo(user.Id, JwtType.Access));
            string refreshToken = _jwtManager.GenerateAndSaveRefreshToken(new JwtInfo(user.Id, JwtType.Refresh));

            return new LoginRs
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}