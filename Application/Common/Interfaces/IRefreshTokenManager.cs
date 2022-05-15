namespace Application.Common.Interfaces
{
    public interface IRefreshTokenManager
    {
        Task SaveTokenAsync(string id, string refreshToken);
        Task<string> ValidateAndRefresh(string userName, string refreshToken);
    }
}