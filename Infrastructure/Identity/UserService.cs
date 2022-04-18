using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationIdentityDbContext _appIdentityDbContext;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationIdentityDbContext appIdentityDbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _appIdentityDbContext = appIdentityDbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task CreateManyWithRoleAsync(List<User> users)
        {
            foreach (User user in users)
            {
                await CreateWithRoleAsync(user);
            }
        }

        public async Task<string> CreateWithRoleAsync(User user)
        {
            ApplicationUser appUser = new()
            {
                UserName = user.UserName
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, GetDefaultPassword());
            if (result.Succeeded)
            {
                throw new Exception
                (
                    string.Join(Environment.NewLine, result.Errors.Select(x => $"Code: {x.Code}. Description: {x.Description}")
                ));
            }

            await _userManager.AddToRoleAsync(appUser, user.RoleName);
            return appUser.Id;
        }

        public async Task<User> GetCurrentUser()
        {
            ClaimsPrincipal? claimsPrincipal = _httpContextAccessor.HttpContext?.User;
            if (claimsPrincipal == null)
            {
                throw new Exception("User is not logged in");
            }

            ApplicationUser appUser = await _userManager.GetUserAsync(claimsPrincipal);
            IList<string> roles = await _userManager.GetRolesAsync(appUser);

            // For now get allow the first role since one user has one role only
            string roleName = roles.First();
            return appUser.ToAppServiceUser(roleName);
        }

        public async Task<List<string>> GetUsernamesStartWith(string startWith)
        {
            return await _appIdentityDbContext.Users
                .Where(x => x.UserName.StartsWith(startWith))
                .Select(x => x.UserName)
                .ToListAsync();
        }

        public async Task<bool> IsHasUserAsync()
        {
            return await _appIdentityDbContext.Users.AnyAsync();
        }

        private string GetDefaultPassword()
        {
            return _configuration.GetValue<string>("DefaultPassword");
        }
    }
}