using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Settings;
using Application.Jwt;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security
{
    public class RefreshTokenManager : IRefreshTokenManager
    {
        private readonly ApplicationIdentityDbContext _appIdentityDbContext;
        private readonly AppSettings _appSettings;
        private readonly IJwtManager _jwtManager;
        public RefreshTokenManager(ApplicationIdentityDbContext appIdentityDbContext, IJwtManager jwtManager, AppSettings appSettings)
        {
            _jwtManager = jwtManager;
            _appSettings = appSettings;
            _appIdentityDbContext = appIdentityDbContext;

        }

        public async Task<string> ValidateAndRefresh(string userName, string refreshToken)
        {
            JwtRefreshToken token = await _appIdentityDbContext.JwtRefreshTokens.FirstOrDefaultAsync(x => x.UserName == userName && x.Token == refreshToken);
            if (token == null || token.ExpireDate < DateTime.Now)
            {
                throw new ServiceException("Invalid token");
            }

            string newRefreshToken = _jwtManager.GenerateToken
            (
                new JwtInfo(userName, JwtType.Refresh, _appSettings.JwtRefreshTokenExpiryDuration)
            );

            using (var transaction = await _appIdentityDbContext.Database.BeginTransactionAsync())
            {
                _appIdentityDbContext.JwtRefreshTokens.Remove(token);

                await SaveTokenAsync(userName, newRefreshToken, false);

                await _appIdentityDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            return newRefreshToken;
        }

        public async Task SaveTokenAsync(string userName, string refreshToken, bool commit = true)
        {
            JwtRefreshToken token = new(userName, refreshToken, _appSettings.JwtRefreshTokenExpiryDuration);
            await _appIdentityDbContext.JwtRefreshTokens.AddAsync(token);

            if (commit)
            {
                await _appIdentityDbContext.SaveChangesAsync();
            }
        }
    }
}