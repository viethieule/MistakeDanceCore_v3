namespace Infrastructure.Security
{
    public class JwtRefreshToken
    {
        public JwtRefreshToken()
        {
        }

        public JwtRefreshToken(string userName, string token, TimeSpan duration)
        {
            UserName = userName;
            Token = token;
            ExpireDate = DateTime.Now.Add(duration);
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}