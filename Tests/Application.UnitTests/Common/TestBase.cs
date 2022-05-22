using Application.Common.Interfaces;
using Application.Users;
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

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<MistakeDanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new MistakeDanceDbContext(options);
        _context.Database.EnsureCreated();

        _userContextMock = new Mock<IUserContext>();
        _userContextMock.SetupGet(x => x.User).Returns(new User
        {
            Id = TEST_USER_ID, UserName = TEST_USER_NAME, RoleName = TEST_USER_ROLE 
        });

        _dtcCollection = new DTCCollection(_context, _userContextMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
