using Application.Common;
using Application.Common.Interfaces;
using Application.Members;
using Application.Memberships;
using Application.Sessions;
using Application.Users;

namespace Application.Registrations
{
    public class CancelRegistrationRq : BaseRequest
    {
        public int RegistrationId { get; set; }
    }

    public class CancelRegistrationRs : BaseResponse
    {
    }

    public class CancelRegistrationService : TransactionalService<CancelRegistrationRq, CancelRegistrationRs>
    {
        private readonly MembershipDTC _membershipDTC;
        private readonly RegistrationDTC _registrationDTC;
        private readonly MemberDTC _memberDTC;
        private readonly SessionDTC _sessionDTC;

        public CancelRegistrationService(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext, MembershipDTC membershipDTC, RegistrationDTC registrationDTC, MemberDTC memberDTC, SessionDTC sessionDTC) : base(mistakeDanceDbContext, userContext)
        {
            _sessionDTC = sessionDTC;
            _memberDTC = memberDTC;
            _registrationDTC = registrationDTC;
            _membershipDTC = membershipDTC;
        }

        protected override async Task<CancelRegistrationRs> RunTransactionalAsync(CancelRegistrationRq rq)
        {
            RegistrationDTO registration = await _registrationDTC.SingleByIdAsync(rq.RegistrationId);

            MemberDTO member = await _memberDTC.SingleByIdAsync(registration.MemberId);

            bool isMember = this.User.RoleName == RoleName.Member;
            if (isMember && registration.MemberId != member.Id)
            {
                throw new Exception();
            }

            SessionDTO session = await _sessionDTC.SingleWithScheduleByIdAsync(registration.SessionId);
            DateTime dateAttending = session.Date.Add(session.StartTime);
            if (isMember && (DateTime.Now >= dateAttending || (dateAttending - DateTime.Now).TotalHours < 1))
            {
                throw new Exception();
            }

            MembershipDTO membership = await _membershipDTC.SingleByMemberIdAsync(registration.MemberId);
            membership.RemainingSessions++;
            await _membershipDTC.UpdateAsync(membership);

            await _registrationDTC.DeleteAsync(registration);
            
            return new CancelRegistrationRs();
        }
    }
}