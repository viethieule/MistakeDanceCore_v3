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

    public class ListPackageByMemberService : BaseService<ListPackageByMemberRq, ListPackageByMemberRs>
    {
        private readonly IUserService _userService;
        private readonly MemberDTC _memberDTC;
        private readonly PackageDTC _packageDTC;
        public ListPackageByMemberService(IUserService userService, MemberDTC memberDTC, PackageDTC packageDTC)
        {
            _packageDTC = packageDTC;
            _memberDTC = memberDTC;
            _userService = userService;
        }

        public override async Task<ListPackageByMemberRs> RunAsync(ListPackageByMemberRq rq)
        {
            UserDTO user = await _userService.GetCurrentUser();
            MemberDTO member = await _memberDTC.SingleByIdAsync(rq.MemberId);
            if (user.RoleName == RoleName.Member && user.Id == member.UserId)
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