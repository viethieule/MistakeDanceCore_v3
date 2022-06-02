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
    protected const string TEST_USER_ID = "00000000-0000-0000-0000-000000000001";
    protected const string TEST_USER_NAME = "testuser";
    protected const string TEST_USER_ROLE = "testuserrole";
    protected readonly MistakeDanceDbContext _context;
    protected readonly DTCCollection _dtcCollection;
    protected readonly Mock<IUserContext> _userContextMock;

    protected const int TEST_BRANCH_1_ID = 1;
    protected const string TEST_BRANCH_1_NAME = "Test branch 1 name";
    protected const int TEST_BRANCH_2_ID = 2;
    protected const string TEST_BRANCH_2_NAME = "Test branch 2 name";
    protected const int TEST_BRANCH_3_ID = 3;
    protected const string TEST_BRANCH_3_NAME = "Test branch 3 name";

    protected const int TEST_TRAINER_1_ID = 1;
    protected const string TEST_TRAINER_1_NAME = "Test trainer 1 name";
    protected const int TEST_TRAINER_2_ID = 2;
    protected const string TEST_TRAINER_2_NAME = "Test trainer 2 name";

    protected const int TEST_CLASS_1_ID = 1;
    protected const string TEST_CLASS_1_NAME = "Test class 1 name";
    protected const int TEST_CLASS_2_ID = 2;
    protected const string TEST_CLASS_2_NAME = "Test class 2 name";

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
            new Branch { Id = TEST_BRANCH_1_ID, Name = TEST_BRANCH_1_NAME },
            new Branch { Id = TEST_BRANCH_2_ID, Name = TEST_BRANCH_2_NAME },
            new Branch { Id = TEST_BRANCH_3_ID, Name = TEST_BRANCH_3_NAME },
        });

        _context.Classes.AddRange(new[]
        {
            new Class { Id = TEST_CLASS_1_ID, Name = TEST_CLASS_1_NAME },
            new Class { Id = TEST_CLASS_2_ID, Name = TEST_CLASS_2_NAME },
        });

        _context.Trainers.AddRange(new[]
        {
            new Trainer { Id = TEST_TRAINER_1_ID, Name = TEST_TRAINER_1_NAME },
            new Trainer { Id = TEST_TRAINER_2_ID, Name = TEST_TRAINER_2_NAME },
        });

        _context.SaveChanges();

        _userContextMock = new Mock<IUserContext>();
        _userContextMock.SetupGet(x => x.User).Returns(new User
        {
            Id = TEST_USER_ID,
            UserName = TEST_USER_NAME,
            RoleName = TEST_USER_ROLE
        });

        _dtcCollection = new DTCCollection(_context, _userContextMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
