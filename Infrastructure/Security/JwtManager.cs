using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Settings;
using Application.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public class JwtManager : IJwtManager
    {
        private readonly string _jwtSigningKey;

        public JwtManager(AppSettings appSettings)
        {
            _jwtSigningKey = appSettings.JwtSigningKey;
        }

        public string GenerateToken(JwtInfo jwtInfo)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, jwtInfo.UserName),
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            string tokenType = jwtToken.Claims.First(x => x.Type == AppClaimTypes.Type).Value;
            if (tokenType != type.ToString())
            {
                throw new SecurityTokenInvalidTypeComparisonException();
            }

            string userName = jwtToken.Claims.First(x => x.Type == AppClaimTypes.UserName).Value;

            return new JwtInfo(userName, type, jwtToken.ValidTo);
        }
    }

    public class SecurityTokenInvalidTypeComparisonException : SecurityTokenException
    {
        public SecurityTokenInvalidTypeComparisonException(string message = "Invalid type comparison") : base(message)
        {
        }
    }
}