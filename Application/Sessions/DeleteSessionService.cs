using Application.Common;
using Application.Common.Interfaces;
using Application.Memberships;
using Application.Packages;
using Application.Registrations;
using Application.Schedules;

namespace Application.Sessions
{
    public class DeleteSessionRq : BaseRequest
    {
        public int SessionId { get; set; }
    }

    public class DeleteSessionRs : BaseResponse
    {
    }

    public class DeleteSessionService : TransactionalService<DeleteSessionRq, DeleteSessionRs>
    {
        private readonly ScheduleDTC _scheduleDTC;
        private readonly SessionDTC _sessionDTC;
        private readonly MembershipDTC _membershipDTC;
        private readonly RegistrationDTC _registrationDTC;
        private readonly DeleteScheduleService _deleteScheduleService;

        public DeleteSessionService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            ScheduleDTC scheduleDTC,
            SessionDTC sessionDTC,
            PackageDTC packageDTC,
            MembershipDTC membershipDTC,
            RegistrationDTC registrationDTC,
            DeleteScheduleService deleteScheduleService) : base(mistakeDanceDbContext)
        {
            _scheduleDTC = scheduleDTC;
            _sessionDTC = sessionDTC;
            _membershipDTC = membershipDTC;
            _registrationDTC = registrationDTC;
            _deleteScheduleService = deleteScheduleService;
        }

        // Delete the session and all following sessions of a schedule
        protected override async Task<DeleteSessionRs> RunTransactionalAsync(DeleteSessionRq rq)
        {
            DeleteSessionRs rs = new DeleteSessionRs();
            SessionDTO sessionDto = await _sessionDTC.SingleByIdAsync(rq.SessionId);
            if (sessionDto.Number == 1)
            {
                await _deleteScheduleService.RunAsync(new DeleteScheduleRq { ScheduleId = sessionDto.ScheduleId });
                
                return rs;
            }

            // Adjust schedule total sessions
            ScheduleDTO scheduleDTO = await _scheduleDTC.SingleByIdAsync(sessionDto.ScheduleId);
            scheduleDTO.TotalSessions = sessionDto.Number - 1;

            List<SessionDTO> sessionDTOs = await _sessionDTC.ListFollowingSessions(sessionDto);
            sessionDTOs.Add(sessionDto);

            List<RegistrationDTO> registrationDTOs = await _registrationDTC.ListShallowBySessionIdsAsync(sessionDTOs.Select(x => x.Id));

            if (registrationDTOs.Count > 0)
            {
                Dictionary<int, int> memberIdAndRemainingSessionToReturn = registrationDTOs
                    .GroupBy(x => x.MemberId)
                    .ToDictionary(x => x.Key, x => x.Count());

                await _membershipDTC.UpdateRemainingSessionsByMemberIds(memberIdAndRemainingSessionToReturn);
            }

            await _sessionDTC.RemoveRangeAsync(sessionDTOs);
            return rs;
        }
    }
}