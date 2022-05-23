using Application.Common;
using Application.Common.Interfaces;
using Application.Members;
using Application.Users;

namespace Application.Packages
{
    public class ListPackageByMemberRq : BaseRequest
    {
        public int MemberId { get; set; }
    }

    public class ListPackageByMemberRs : BaseResponse
    {
        public List<PackageDTO> Packages { get; set; }
    }

    public class ListPackageByMemberService : AuthenticatedService<ListPackageByMemberRq, ListPackageByMemberRs>
    {
        private readonly MemberDTC _memberDTC;
        private readonly PackageDTC _packageDTC;
        public ListPackageByMemberService(IUserContext userContext, MemberDTC memberDTC, PackageDTC packageDTC) : base(userContext)
        {
            _packageDTC = packageDTC;
            _memberDTC = memberDTC;
        }

        protected override async Task<ListPackageByMemberRs> DoRunAsync(ListPackageByMemberRq rq)
        {
            MemberDTO member = await _memberDTC.SingleByIdAsync(rq.MemberId);
            if (this.User.RoleName == RoleName.Member && this.User.Id == member.UserId)
            {
                throw new Exception("Not found");
            }

            List<PackageDTO> packageDTOs = await _packageDTC.ListByMemberIdAsync(rq.MemberId);
            return new ListPackageByMemberRs
            {
                Packages = packageDTOs
            };
        }
    }
}