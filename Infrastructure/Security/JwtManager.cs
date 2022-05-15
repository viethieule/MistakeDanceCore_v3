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

        private readonly string _jwtSigningKey;

        public JwtManager(IConfiguration configuration)
        {
            _jwtSigningKey = configuration[CONFIG_NAME_JWT_SIGNING_KEY];
        }

        public string GenerateToken(JwtInfo jwtInfo)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(AppClaimTypes.UserName, jwtInfo.UserName),
                new Claim(AppClaimTypes.Type, jwtInfo.Type.ToString())
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSigningKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = jwtInfo.Expires,
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public JwtInfo Validate(string token, JwtType type)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSigningKey))
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            string tokenType = jwtToken.Claims.First(x => x.Type == AppClaimTypes.Type).Value;
            if (tokenType == type.ToString())
            {
                throw new SecurityTokenInvalidTypeComparisonException("Invalid type comparison");
            }

            string userName = jwtToken.Claims.First(x => x.Type == AppClaimTypes.UserName).Value;

            return new JwtInfo(userName, type, jwtToken.ValidTo);
        }
    }

    public class SecurityTokenInvalidTypeComparisonException : SecurityTokenException
    {
        public SecurityTokenInvalidTypeComparisonException(string message) : base(message)
        {
        }
    }
}