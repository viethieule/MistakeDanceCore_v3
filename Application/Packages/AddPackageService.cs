using Application.Common;
using Application.Common.Interfaces;
using Application.Memberships;

namespace Application.Packages
{
    public class AddPackageRq : BaseRequest
    {
        public PackageDTO Package { get; set; }
    }

    public class AddPackageRs : BaseResponse
    {
    }

    public class AddPackageService : TransactionalService<AddPackageRq, AddPackageRs>
    {
        private readonly PackageDTC _packageDTC;
        private readonly MembershipDTC _membershipDTC;
        public AddPackageService(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext, PackageDTC packageDTC, MembershipDTC membershipDTC) : base(mistakeDanceDbContext, userContext)
        {
            _membershipDTC = membershipDTC;
            _packageDTC = packageDTC;
        }

        protected override async Task<AddPackageRs> RunTransactionalAsync(AddPackageRq rq)
        {
            PackageDTO packageDTO = rq.Package;
            await _packageDTC.CreateAsync(packageDTO);

            MembershipDTO membershipDTO = await _membershipDTC.SingleByMemberIdAsync(packageDTO.MemberId);

            if (membershipDTO.ExpiryDate >= DateTime.Now.Date && membershipDTO.RemainingSessions > 0)
            {
                DateTime expiryDate = membershipDTO.ExpiryDate.AddMonths(packageDTO.Months);
                membershipDTO.ExpiryDate = expiryDate;
            }
            else
            {
                // Since it is expired, reset remaining session to 0
                membershipDTO.RemainingSessions = 0;

                DateTime expiryDate = DateTime.Now.AddMonths(packageDTO.Months);
                membershipDTO.ExpiryDate = expiryDate;
            }

            membershipDTO.RemainingSessions += packageDTO.NumberOfSessions;
            await _membershipDTC.UpdateAsync(membershipDTO);

            return new AddPackageRs();
        }
    }
}