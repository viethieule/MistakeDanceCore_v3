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
        private readonly SessionDTC _sessionDTC;
        public ListSessionsService(SessionDTC sessionDTC)
        {
            _sessionDTC = sessionDTC;
        }

        protected override async Task<ListSessionsRs> RunAsync(ListSessionsRq rq)
        {
            List<SessionDTO> sessions = await _sessionDTC.ListAsync(rq.Start, rq.End);

            return new ListSessionsRs
            {
                Sessions = sessions
            };
        }
    }
}