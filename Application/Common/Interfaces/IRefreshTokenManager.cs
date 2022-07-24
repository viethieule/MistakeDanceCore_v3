using Application.Jwt;

namespace Application.Common.Interfaces
{
    public interface IRefreshTokenManager
    {
        Task SaveTokenAsync(string id, string refreshToken, bool commit = true);
        Task<string> ValidateAndRefresh(string userName, string refreshToken, JwtInfo newRefreshTokenInfo);
    }
}