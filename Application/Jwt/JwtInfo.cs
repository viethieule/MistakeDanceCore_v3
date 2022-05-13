namespace Application.Jwt
{
    public class JwtInfo
    {
        public JwtInfo(string userId, JwtType type)
        {
            UserId = userId;
            Type = type;
        }
        
        public string UserId { get; set; }
        public JwtType Type { get; set; }
    }

    public enum JwtType
    {
        Access,
        Refresh
    }
}