using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.DefaultPackages;
using Application.Members;
using Application.Memberships;
using Application.Packages;
using Application.UnitTests.Common;
using Application.Users;
using Moq;
using Xunit;

namespace Application.UnitTests.Members;
public class CreateMemberTests : TestBase
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly IUsernameGenerator _usernameGenerator;
    public CreateMemberTests() : base()
    {
        _userServiceMock = new Mock<IUserService>();
        _userServiceMock.Setup(x => x.CreateWithRoleAsync(It.IsAny<User>())).ReturnsAsync(Guid.NewGuid().ToString());
        _userServiceMock.Setup(x => x.GetUsernamesStartWith(It.IsAny<string>())).ReturnsAsync(new List<string>());

        _usernameGenerator = new UsernameGenerator(_userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_GiveCorrectInput_CreateMemberSuccessfully_WithRelatedData()
    {
        CreateMemberRq createRq = new CreateMemberRq
        {
            Member = new MemberDTO
            {
                FullName = "Test Member",
                BranchId = TestConstants.BRANCH_1_ID,
                PhoneNumber = "0123456789",
            },
            Package = new PackageDTO
            {
                DefaultPackageId = TestConstants.DEFAULT_PACKAGE_1_ID
            }
        };

        CreateMemberService createMemberService = new CreateMemberService(
            _context, _userContextMock.Object, _userServiceMock.Object, _usernameGenerator,
            _dtcCollection.MemberDTC, _dtcCollection.PackageDTC, _dtcCollection.MembershipDTC);

        await createMemberService.RunAsync(createRq);

        MemberDTO member = await _dtcCollection.MemberDTC.SingleByIdAsync(createRq.Member.Id);
        Assert.Equal(createRq.Member.FullName, member.FullName);
        Assert.Equal(createRq.Member.FullName.NormalizeVietnameseDiacritics(), member.NormalizedFullName);
        Assert.Equal(createRq.Member.BranchId, member.BranchId);
        Assert.Equal(createRq.Member.PhoneNumber, member.PhoneNumber);
        Assert.Equal("member.test", member.UserName);
        Assert.True(Guid.TryParse(member.UserId, out _));
        Assert.Equal(TestConstants.TEST_USER_NAME, member.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, member.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, member.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, member.UpdatedDate.Date);

        List<PackageDTO> packages = await _dtcCollection.PackageDTC.ListByMemberIdAsync(member.Id);
        Assert.NotEmpty(packages);
        PackageDTO package = packages.Single();
        Assert.Equal(createRq.Package.NumberOfSessions, package.NumberOfSessions);
        Assert.True(package.DefaultPackageId.HasValue);
        Assert.Equal(TestConstants.DEFAULT_PACKAGE_1_ID, package.DefaultPackageId!.Value);
        
        DefaultPackageDTO defaultPackage = await _dtcCollection.DefaultPackageDTC.SingleByIdAsync(package.DefaultPackageId!.Value);
        Assert.Equal(defaultPackage.Price, package.Price);
        Assert.Equal(defaultPackage.Months, package.Months);
        Assert.Equal(defaultPackage.NumberOfSessions, package.NumberOfSessions);
        Assert.Equal(createRq.Member.BranchId, package.BranchRegisteredId);
        Assert.Equal(TestConstants.TEST_USER_NAME, package.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, package.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, package.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, package.UpdatedDate.Date);

        MembershipDTO membership = await _dtcCollection.MembershipDTC.SingleByMemberIdAsync(member.Id);
        Assert.Equal(package.NumberOfSessions, membership.RemainingSessions);
        Assert.Equal(DateTime.Now.AddMonths(package.Months).Date, membership.ExpiryDate.Date);
        Assert.Equal(TestConstants.TEST_USER_NAME, membership.CreatedBy);
        Assert.Equal(TestConstants.TEST_USER_NAME, membership.UpdatedBy);
        Assert.Equal(DateTime.Now.Date, membership.CreatedDate.Date);
        Assert.Equal(DateTime.Now.Date, membership.UpdatedDate.Date);
    }
}