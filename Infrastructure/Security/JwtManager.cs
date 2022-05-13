using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public class JwtManager : IJwtManager
    {
        private const string CONFIG_NAME_JWT_SIGNING_KEY = "JwtSigningKey";
        private const string CONFIG_NAME_JWT_ACCESS_EXPIRES_DURATION = "JwtAccessExpireDuration";

        private readonly string _jwtSigningKey;
        private readonly IConfiguration _configuration;

        public JwtManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSigningKey = configuration[CONFIG_NAME_JWT_SIGNING_KEY];
        }

        public string GenerateToken(JwtInfo jwtInfo)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(AppClaimTypes.Id, jwtInfo.UserId),
                new Claim(AppClaimTypes.Type, jwtInfo.Type.ToString())
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSigningKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(int.Parse(_configuration[CONFIG_NAME_JWT_ACCESS_EXPIRES_DURATION])),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateAndSaveRefreshToken(JwtInfo jwtInfo)
        {
            string refreshToken = this.GenerateToken(jwtInfo);
            return refreshToken;
        }
    }
}