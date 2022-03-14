using Application.Common;
using Application.Common.Interfaces;
using Application.Memberships;
using Application.Packages;

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
        private readonly PackageDTC _packageDTC;
        private readonly RegistrationDTC _registrationDTC;
        
        public CreateRegistrationService(IMistakeDanceDbContext mistakeDanceDbContext, MembershipDTC membershipDTC, PackageDTC packageDTC, RegistrationDTC registrationDTC) : base(mistakeDanceDbContext)
        {
            this._registrationDTC = registrationDTC;
            this._packageDTC = packageDTC;
            this._membershipDTC = membershipDTC;
        }

        protected override async Task<CreateRegistrationRs> RunTransactionalAsync(CreateRegistrationRq rq)
        {
            MembershipDTO membership = await _membershipDTC.SingleByMemberIdAsync(rq.MemberId);
            if (membership.IsExpired)
            {
                throw new Exception();
            }

            return new CreateRegistrationRs();
        }
    }
}