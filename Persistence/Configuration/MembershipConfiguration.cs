using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Configuration
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(x => x.MemberId);
        }
    }
}