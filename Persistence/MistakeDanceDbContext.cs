using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class MistakeDanceDbContext : DbContext, IMistakeDanceDbContext
    {
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<DefaultPackage> DefaultPackages { get; set; }
    }
}