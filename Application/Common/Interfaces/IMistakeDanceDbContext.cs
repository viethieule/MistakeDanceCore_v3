using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces
{
    public interface IMistakeDanceDbContext
    {
        DbSet<Schedule> Schedules { get; set; }
        DbSet<Branch> Branches { get; set; }
        DbSet<Class> Classes { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<Trainer> Trainers { get; set; }
        DbSet<Registration> Registrations { get; set; }
        DbSet<Member> Members { get; set; }
        DbSet<Membership> Memberships { get; set; }
        DbSet<Package> Packages { get; set; }
        DbSet<DefaultPackage> DefaultPackages { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}