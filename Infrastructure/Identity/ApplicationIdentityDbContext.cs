using Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<JwtRefreshToken> JwtRefreshTokens { get; set; }
        public ApplicationIdentityDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}