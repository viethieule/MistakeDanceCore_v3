using Microsoft.Extensions.Configuration;

namespace Application.Common.Settings
{
    public static class SettingsUtils
    {
        public static TimeSpan ReadTimeSpan(IConfiguration configuration, string key)
        {
            string s = ReadString(configuration, key);
            return TimeSpan.Parse(s);
        }

        public static string ReadString(IConfiguration configuration, string key)
        {
            string value = configuration[key];
            return !string.IsNullOrEmpty(value) ? value.Trim() : string.Empty;
        }
    }
}