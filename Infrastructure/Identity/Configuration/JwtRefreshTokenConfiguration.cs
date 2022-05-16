using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Identity.Configuration
{
    public class JwtRefreshTokenConfiguration : IEntityTypeConfiguration<JwtRefreshToken>
    {
        public void Configure(EntityTypeBuilder<JwtRefreshToken> builder)
        {
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.Token).IsRequired();
            builder.Property(x => x.ExpireDate).IsRequired();
        }
    }
}