using Application.Common;
using Application.Common.Interfaces;
using Application.Memberships;
using Application.Packages;
using Application.Users;

namespace Application.Members
{
    public class CreateMemberRq : BaseRequest
    {
        public MemberDTO Member { get; set; }
        public PackageDTO Package { get; set; }
    }

    public class CreateMemberRs : BaseResponse
    {
        public int Id { get; set; }
    }

    public class CreateMemberService : TransactionalService<CreateMemberRq, CreateMemberRs>
    {
        private readonly IUserService _userService;
        private readonly IUsernameGenerator _usernameGenerator;
        private readonly MemberDTC _memberDTC;
        private readonly PackageDTC _packageDTC;
        private readonly MembershipDTC _membershipDTC;

        public CreateMemberService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            IUserService userService,
            IUsernameGenerator usernameGenerator,
            MemberDTC memberDTC,
            PackageDTC packageDTC,
            MembershipDTC membershipDTC) : base(mistakeDanceDbContext)
        {
            _userService = userService;
            _usernameGenerator = usernameGenerator;
            _memberDTC = memberDTC;
            _packageDTC = packageDTC;
            _membershipDTC = membershipDTC;
        }

        protected override async Task<CreateMemberRs> RunTransactionalAsync(CreateMemberRq rq)
        {
            MemberDTO memberDTO = rq.Member;
            UserDTO user = new UserDTO
            {
                UserName = await _usernameGenerator.Generate(memberDTO.FullName),
                RoleName = RoleName.Member
            };

            int userId = await _userService.CreateWithRoleAsync(user);
            memberDTO.UserId = userId;
            memberDTO.UserName = user.UserName;

            await _memberDTC.CreateAsync(memberDTO);

            PackageDTO packageDTO = rq.Package;
            packageDTO.MemberId = memberDTO.Id;
            await _packageDTC.CreateAsync(packageDTO);

            await _membershipDTC.CreateAsync(new MembershipDTO
            {
                MemberId = memberDTO.Id,
                RemainingSessions = packageDTO.RemainingSessions,
                ExpiryDate = packageDTO.ExpiryDate.Value
            });

            return new CreateMemberRs
            {
                Id = memberDTO.Id
            };
        }
    }
}