using Application.Common;
using Application.Common.Interfaces;
using Domain;

namespace Application.SeedData
{
    public class SeedDataRq : BaseRequest
    {
    }

    public class SeedDataRs : BaseResponse
    {
    }

    public class SeedDataService : BaseService<SeedDataRq, SeedDataRs>
    {
        private readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        public SeedDataService(IMistakeDanceDbContext mistakeDanceDbContext)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;
        }

        public override async Task<SeedDataRs> RunAsync(SeedDataRq rq)
        {
            bool shouldSeed = false;
            if (!_mistakeDanceDbContext.Branches.Any())
            {
                shouldSeed = true;
                _mistakeDanceDbContext.Branches.AddRange(new List<Branch>
                {
                    new() { Name = "Phú Nhuận", Abbreviation = "PN" },
                    new() { Name = "Lê Văn Sỹ", Abbreviation = "LVS" },
                    new() { Name = "Quận 10", Abbreviation = "Q10" },
                });
            }

            if (!_mistakeDanceDbContext.DefaultPackages.Any())
            {
                shouldSeed = true;
                _mistakeDanceDbContext.DefaultPackages.AddRange(new List<DefaultPackage>
                {
                    new() { NumberOfSessions = 8, Months = 2, Price = 700000 },
                    new() { NumberOfSessions = 16, Months = 3, Price = 1280000 },
                    new() { NumberOfSessions = 24, Months = 5, Price = 1850000 },
                    new() { NumberOfSessions = 50, Months = 8, Price = 3500000 },
                });
            }

            if (shouldSeed)
            {
                await _mistakeDanceDbContext.SaveChangesAsync();
            }

            return new SeedDataRs();
        }
    }
}