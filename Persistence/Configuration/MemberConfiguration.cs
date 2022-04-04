using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configuration
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder
                .HasMany(x => x.Packages)
                .WithOne(x => x.Member)
                .HasForeignKey(x => x.MemberId)
                .IsRequired();

            builder
                .HasOne(x => x.Membership)
                .WithOne()
                .HasForeignKey<Membership>(x => x.MemberId)
                .IsRequired();

            builder
                .HasMany(x => x.Registrations)
                .WithOne()
                .HasForeignKey(x => x.MemberId)
                .IsRequired();

            builder
                .HasOne(x => x.Branch)
                .WithMany()
                .IsRequired();

            builder.Property(x => x.FullName).IsRequired();
            builder.Property(x => x.NormalizedFullName).IsRequired();
            builder.Property(x => x.PhoneNumber).IsRequired();
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}