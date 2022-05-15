namespace Application.Jwt
{
    public interface IJwtManager
    {
        string GenerateToken(JwtInfo jwtInfo);
        JwtInfo Validate(string refreshToken, JwtType type);
    }
}