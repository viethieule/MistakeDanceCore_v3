namespace Application.Jwt
{
    public interface IJwtManager
    {
        string GenerateToken(JwtInfo jwtInfo);
        string GenerateAndSaveRefreshToken(JwtInfo jwtInfo);
    }
}