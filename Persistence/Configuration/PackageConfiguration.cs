using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configuration
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder
                .HasOne(x => x.BranchRegistered)
                .WithMany()
                .HasForeignKey(x => x.BranchRegisteredId)
                .IsRequired();

            builder
                .HasOne(x => x.DefaultPackage)
                .WithMany()
                .HasForeignKey(x => x.DefaultPackageId);

            builder.Property(x => x.Months).IsRequired();
            builder.Property(x => x.NumberOfSessions).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}