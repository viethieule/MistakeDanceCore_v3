using Application.Common;
using Application.Common.Interfaces;
using Application.Memberships;
using Application.Packages;
using Application.Registrations;
using Application.Sessions;

namespace Application.Schedules
{
    public class DeleteScheduleRq : BaseRequest
    {
        public int ScheduleId { get; set; }
    }

    public class DeleteScheduleRs : BaseResponse
    {
    }

    public class DeleteScheduleService : TransactionalService<DeleteScheduleRq, DeleteScheduleRs>
    {
        private readonly ScheduleDTC _scheduleDTC;
        private readonly SessionDTC _sessionDTC;
        private readonly RegistrationDTC _registrationDTC;
        private readonly MembershipDTC _membershipDTC;

        public DeleteScheduleService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            ScheduleDTC scheduleDTC,
            SessionDTC sessionDTC,
            RegistrationDTC registrationDTC,
            MembershipDTC membershipDTC) : base(mistakeDanceDbContext)
        {
            _scheduleDTC = scheduleDTC;
            _sessionDTC = sessionDTC;
            _registrationDTC = registrationDTC;
            _membershipDTC = membershipDTC;
        }

        protected override async Task<DeleteScheduleRs> RunTransactionalAsync(DeleteScheduleRq rq)
        {
            ScheduleDTO scheduleDTO = await _scheduleDTC.SingleByIdAsync(rq.ScheduleId);
            List<SessionDTO> sessionDTOs = await _sessionDTC.ListShallowByScheduleIdAsync(rq.ScheduleId);
            List<RegistrationDTO> registrationDTOs = await _registrationDTC.ListShallowByScheduleIdAsync(rq.ScheduleId);

            if (registrationDTOs.Count > 0)
            {
                Dictionary<int, int> memberIdAndRemainingSessionToReturn = registrationDTOs
                    .GroupBy(x => x.MemberId)
                    .ToDictionary(x => x.Key, x => x.Count());

                await _membershipDTC.UpdateRemainingSessionsByMemberIds(memberIdAndRemainingSessionToReturn);
            }

            await _registrationDTC.DeleteRangeAsync(registrationDTOs);
            await _sessionDTC.DeleteRangeAsync(sessionDTOs);
            await _scheduleDTC.DeleteAsync(scheduleDTO);
            
            return new DeleteScheduleRs();
        }
    }
}