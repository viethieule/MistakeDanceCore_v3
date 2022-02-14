using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IMistakeDanceDbContext
    {
        DbSet<Schedule> Schedules { get; set; }
        DbSet<Branch> Branches { get; set; }
        DbSet<Class> Classes { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<Trainer> Trainers { get; set; }
    }
}