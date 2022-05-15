namespace Application.Jwt
{
    public class JwtInfo
    {
        public JwtInfo(string userName, JwtType type, TimeSpan duration)
        {
            UserName = userName;
            Type = type;
            Expires = DateTime.UtcNow.Add(duration);
        }

        public JwtInfo(string userName, JwtType type, DateTime expires)
        {
            UserName = userName;
            Type = type;
            Expires = expires;
        }
        
        public string UserName { get; set; }
        public JwtType Type { get; set; }
        public DateTime Expires { get; init; }
    }

    public enum JwtType
    {
        Access,
        Refresh
    }
}