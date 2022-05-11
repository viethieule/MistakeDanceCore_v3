using Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Persistence;

namespace Application.UnitTests.Common;

public class TestBase : IDisposable
{
    protected readonly MistakeDanceDbContext _context;
    protected readonly DTCCollection _dtcCollection;

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<MistakeDanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new MistakeDanceDbContext(options);
        _context.Database.EnsureCreated();

        _dtcCollection = new DTCCollection(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
