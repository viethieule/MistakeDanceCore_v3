using Application.Common;
using Application.Common.Interfaces;
using Application.Members;
using Application.Memberships;
using Application.Packages;
using Application.Users;

namespace Application.Registrations
{
    public class CreateRegistrationRq : BaseRequest
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
    }

    public class CreateRegistrationRs : BaseResponse
    {
    }

    public class CreateRegistrationService : TransactionalService<CreateRegistrationRq, CreateRegistrationRs>
    {
        private readonly MembershipDTC _membershipDTC;
        private readonly RegistrationDTC _registrationDTC;
        private readonly MemberDTC _memberDTC;

        public CreateRegistrationService(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext, MembershipDTC membershipDTC, RegistrationDTC registrationDTC, MemberDTC memberDTC) : base(mistakeDanceDbContext, userContext)
        {
            _memberDTC = memberDTC;
            _registrationDTC = registrationDTC;
            _membershipDTC = membershipDTC;
        }

        protected override async Task<CreateRegistrationRs> RunTransactionalAsync(CreateRegistrationRq rq)
        {
            MemberDTO member = await _memberDTC.SingleByIdAsync(rq.MemberId);

            if (this.User.RoleName == RoleName.Member && rq.MemberId != member.Id)
            {
                throw new Exception();
            }

            MembershipDTO membership = await _membershipDTC.SingleByMemberIdAsync(rq.MemberId);
            if (membership.IsExpired)
            {
                throw new Exception();
            }

            membership.RemainingSessions += -1;

            RegistrationDTO registrationDTO = new RegistrationDTO
            {
                SessionId = rq.SessionId,
                MemberId = rq.MemberId
            };
            await _registrationDTC.CreateAsync(registrationDTO);

            return new CreateRegistrationRs();
        }
    }
}