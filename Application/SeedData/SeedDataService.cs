using Application.Common;
using Application.Common.Interfaces;
using Application.Users;
using Domain;
using Microsoft.EntityFrameworkCore;

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
        private readonly IUserService _userService;
        public SeedDataService(IMistakeDanceDbContext mistakeDanceDbContext, IUserService userService)
        {
            _userService = userService;
            _mistakeDanceDbContext = mistakeDanceDbContext;
        }

        public override async Task<SeedDataRs> RunAsync(SeedDataRq rq)
        {
            bool shouldSeed = false;
            if (!(await _mistakeDanceDbContext.Branches.AnyAsync()))
            {
                shouldSeed = true;
                _mistakeDanceDbContext.Branches.AddRange(new List<Branch>
                {
                    new() { Name = "Phú Nhuận", Abbreviation = "PN", CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                    new() { Name = "Lê Văn Sỹ", Abbreviation = "LVS", CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                    new() { Name = "Quận 10", Abbreviation = "Q10", CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                });
            }

            if (!(await _mistakeDanceDbContext.DefaultPackages.AnyAsync()))
            {
                shouldSeed = true;
                _mistakeDanceDbContext.DefaultPackages.AddRange(new List<DefaultPackage>
                {
                    new() { NumberOfSessions = 8, Months = 2, Price = 700000, CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                    new() { NumberOfSessions = 16, Months = 3, Price = 1280000, CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                    new() { NumberOfSessions = 24, Months = 5, Price = 1850000, CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                    new() { NumberOfSessions = 50, Months = 8, Price = 3500000, CreatedBy = "SeedDataService", CreatedDate = DateTime.Now, UpdatedBy = "SeedDataService", UpdatedDate = DateTime.Now },
                });
            }

            if (shouldSeed)
            {
                await _mistakeDanceDbContext.SaveChangesAsync();
            }

            return new SeedDataRs();
        }

        private async Task SeedIdentityData()
        {
            if (!(await _userService.IsHasUserAsync()))
            {
                await _userService.CreateManyWithRoleAsync(new List<User>
                {
                    new() { UserName = "admin", RoleName = RoleName.Admin },
                    new() { UserName = "receptionist", RoleName = RoleName.Receptionist },
                    new() { UserName = "collaborator", RoleName = RoleName.Collaborator },
                });
            }
        }
    }
}