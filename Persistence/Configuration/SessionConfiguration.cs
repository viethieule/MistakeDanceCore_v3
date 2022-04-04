using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configuration
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder
                .HasMany(x => x.Registrations)
                .WithOne()
                .HasForeignKey(x => x.SessionId)
                .IsRequired();

            builder.Property(x => x.Number).IsRequired();
            builder.Property(x => x.Date).IsRequired();
        }
    }
}