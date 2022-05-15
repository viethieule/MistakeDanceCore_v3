using Microsoft.Extensions.Configuration;

namespace Application.Common.Settings
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            this.UserDefaultPassword = SettingsUtils.ReadString(configuration, "Identity:MemberDefaultPassword");   
            this.JwtAccessTokenExpiryDuration = SettingsUtils.ReadTimeSpan(configuration, "Jwt:JwtAccessTokenExpiryDuration");
            this.JwtRefreshTokenExpiryDuration = SettingsUtils.ReadTimeSpan(configuration, "Jwt:JwtRefreshTokenExpiryDuration");
            this.JwtSigningKey = SettingsUtils.ReadString(configuration, "Jwt:JwtSigningKey");
        }

        public string UserDefaultPassword { get; set; }
        public TimeSpan JwtAccessTokenExpiryDuration { get; private set; }
        public TimeSpan JwtRefreshTokenExpiryDuration { get; private set; }
        public string JwtSigningKey { get; private set; }
    }
}