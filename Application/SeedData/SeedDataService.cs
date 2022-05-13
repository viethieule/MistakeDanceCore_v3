using Application.Common;
using Application.Common.Interfaces;
using Application.Users;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<SeedDataService> _logger;
        private readonly IRoleService _roleService;
        public SeedDataService(IMistakeDanceDbContext mistakeDanceDbContext, IUserService userService, IRoleService roleService, ILogger<SeedDataService> logger)
        {
            _logger = logger;
            _userService = userService;
            _roleService = roleService;
            _mistakeDanceDbContext = mistakeDanceDbContext;
        }

        protected override async Task<SeedDataRs> DoRunAsync(SeedDataRq rq)
        {
            _logger.LogInformation("Start seeding data");

            var seedAppDataTask = SeedApplicationData();
            var seedIdentityDataTask = SeedIdentityData();
            await Task.WhenAll(seedAppDataTask, seedIdentityDataTask);

            return new SeedDataRs();
        }

        private async Task SeedApplicationData()
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
                _logger.LogInformation("Start seeding application data...");
                await _mistakeDanceDbContext.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("Application data exists: No seeding");
            }
        }

        private async Task SeedIdentityData()
        {
            bool shouldSeed = false;

            if (!(await _roleService.IsHasRoleAsync()))
            {
                shouldSeed = true;
                await _roleService.CreateRolesAsync
                (
                    RoleName.Admin,
                    RoleName.Collaborator,
                    RoleName.Member,
                    RoleName.Receptionist
                );
            }

            if (!(await _userService.IsHasUserAsync()))
            {
                shouldSeed = true;
                await _userService.CreateManyWithRoleAsync(new List<User>
                {
                    new() { UserName = "admin", RoleName = RoleName.Admin },
                    new() { UserName = "receptionist", RoleName = RoleName.Receptionist },
                    new() { UserName = "collaborator", RoleName = RoleName.Collaborator },
                });
            }

            if (shouldSeed)
            {
                _logger.LogInformation("Start seeding identity data...");
            }
            else
            {
                _logger.LogInformation("Identity data exists: No seeding");
            }
        }
    }
}