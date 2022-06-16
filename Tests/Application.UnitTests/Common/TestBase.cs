using Application.Common.Interfaces;
using Application.Users;
using Domain;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Persistence;

namespace Application.UnitTests.Common;

public class TestBase : IDisposable
{
    protected readonly MistakeDanceDbContext _context;
    protected readonly DTCCollection _dtcCollection;
    protected readonly Mock<IUserContext> _userContextMock;

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<MistakeDanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new MistakeDanceDbContext(options);
        _context.Database.EnsureCreated();

        _context.Branches.AddRange(new[]
        {
            new Branch { Id = TestConstants.BRANCH_1_ID, Name = TestConstants.BRANCH_1_NAME },
            new Branch { Id = TestConstants.BRANCH_2_ID, Name = TestConstants.BRANCH_2_NAME },
            new Branch { Id = TestConstants.BRANCH_3_ID, Name = TestConstants.BRANCH_3_NAME },
        });

        _context.Classes.AddRange(new[]
        {
            new Class { Id = TestConstants.CLASS_1_ID, Name = TestConstants.CLASS_1_NAME },
            new Class { Id = TestConstants.CLASS_2_ID, Name = TestConstants.CLASS_2_NAME },
        });

        _context.Trainers.AddRange(new[]
        {
            new Trainer { Id = TestConstants.TRAINER_1_ID, Name = TestConstants.TRAINER_1_NAME },
            new Trainer { Id = TestConstants.TRAINER_2_ID, Name = TestConstants.TRAINER_2_NAME },
        });

        _context.Members.Add(new Member
        {
            Id = TestConstants.MEMBER_1_ID,
            FullName = "Test member 1",
            NormalizedFullName = "Test member 1",
            PhoneNumber = "0123456789",
            Birthdate = DateTime.Now.AddYears(-20),
            BranchId = TestConstants.BRANCH_1_ID,
            UserId = "1",
            UserName = "testmember1",
            Membership = new Membership
            {
                RemainingSessions = 1000,
                ExpiryDate = DateTime.MaxValue
            },
            Packages = new List<Package>
            {
                new Package
                {
                    Id = TestConstants.MEMBER_1_PACKAGE_ID,
                    NumberOfSessions = 1000,
                    Price = 1000000,
                    Months = 100,
                    BranchRegisteredId = TestConstants.BRANCH_1_ID
                }
            }
        });

        _context.DefaultPackages.Add(new DefaultPackage
        {
            Id = TestConstants.DEFAULT_PACKAGE_1_ID,
            NumberOfSessions = 100,
            Price = 1000000,
            Months = 10
        });

        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        _userContextMock = new Mock<IUserContext>();
        _userContextMock.SetupGet(x => x.User).Returns(new User
        {
            Id = TestConstants.TEST_USER_ID,
            UserName = TestConstants.TEST_USER_NAME,
            RoleName = TestConstants.TEST_USER_ROLE
        });

        _dtcCollection = new DTCCollection(_context, _userContextMock.Object);
    }

    public virtual void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
