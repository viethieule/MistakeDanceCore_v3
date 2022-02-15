using Application.Common;
using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Sessions
{
    public class ListSessionsRq : BaseRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class ListSessionsRs : BaseResponse
    {
        public List<SessionDTO> Sessions { get; set; }
    }

    public class ListSessionsService : BaseService<ListSessionsRq, ListSessionsRs>
    {
        public ListSessionsService(IMistakeDanceDbContext mistakeDanceDbContext, IMapper mapper) : base(mistakeDanceDbContext, mapper)
        {
        }

        public override async Task<ListSessionsRs> RunAsync(ListSessionsRq rq)
        {
            List<SessionDTO> sessions = await _mistakeDanceDbContext.Sessions
                .Where(x => x.Date <= rq.End && x.Date >= rq.Start)
                .Include(x => x.Schedule).ThenInclude(x => x.Branch)
                .Include(x => x.Schedule).ThenInclude(x => x.Trainer)
                .Include(x => x.Schedule).ThenInclude(x => x.Class)
                .Include(x => x.Registrations)
                .ProjectTo<SessionDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new ListSessionsRs
            {
                Sessions = sessions
            };
        }
    }
}