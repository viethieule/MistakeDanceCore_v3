using Microsoft.Extensions.Configuration;

namespace Application.Common.Settings
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            this.UserDefaultPassword = SettingsUtils.ReadString(configuration, "Identity:UserDefaultPassword");   
            this.JwtAccessTokenExpiryDuration = SettingsUtils.ReadTimeSpan(configuration, "Jwt:AccessTokenExpiryDuration");
            this.JwtRefreshTokenExpiryDuration = SettingsUtils.ReadTimeSpan(configuration, "Jwt:RefreshTokenExpiryDuration");
            this.JwtSigningKey = SettingsUtils.ReadString(configuration, "Jwt:SigningKey");
        }

        public string UserDefaultPassword { get; set; }
        public TimeSpan JwtAccessTokenExpiryDuration { get; private set; }
        public TimeSpan JwtRefreshTokenExpiryDuration { get; private set; }
        public string JwtSigningKey { get; private set; }
    }
}