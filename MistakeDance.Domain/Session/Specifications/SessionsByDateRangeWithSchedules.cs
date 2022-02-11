using Ardalis.Specification;

namespace MistakeDance.Domain.Specifications
{
    public class SessionsByDateRangeWithSchedules : Specification<Session>
    {
        public SessionsByDateRangeWithSchedules(DateTime start, DateTime end)
        {
            Query
               .Where(x => x.Date >= start && x.Date <= end)
               .Include(x => x.Schedule).ThenInclude(x => x.Branch)
               .Include(x => x.Schedule).ThenInclude(x => x.Trainer)
               .Include(x => x.Schedule).ThenInclude(x => x.Class);
        }
    }
}