using System.Globalization;
using Application.Common;
using FluentValidation.Results;

namespace Application.Sessions
{
    public class ListSessionsRq : BaseRequest
    {
        public DateTime Start { get; set; }
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

        protected override ValidationResult Validate(ListSessionsRq rq)
        {
            return SessionValidators.ListRq.Validate(rq);
        }

        protected override async Task<ListSessionsRs> DoRunAsync(ListSessionsRq rq)
        {
            DateTime start = rq.Start.Date;
            DateTime end = rq.Start.AddDays(7).AddSeconds(-1);

            var rs = new ListSessionsRs();
            rs.Sessions = await _sessionDTC.ListAsync(start, end);

            return rs;
        }
    }
}