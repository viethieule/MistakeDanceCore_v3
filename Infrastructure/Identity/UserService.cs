using Application.Common.Interfaces;
using Application.Common.Settings;
using Application.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationIdentityDbContext _appIdentityDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppSettings _appSettings;

        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationIdentityDbContext appIdentityDbContext, AppSettings appSettings, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _appSettings = appSettings;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appIdentityDbContext = appIdentityDbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<bool> CheckPasswordSigninAsync(string username, string password)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            return result.Succeeded;
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
            ApplicationUser appUser = new(user.UserName);

            IdentityResult result = await _userManager.CreateAsync(appUser, GetDefaultPassword());
            if (!result.Succeeded)
            {
                throw new Exception
                (
                    string.Join(Environment.NewLine, result.Errors.Select(x => $"Code: {x.Code}. Description: {x.Description}")
                ));
            }

            await _userManager.AddToRoleAsync(appUser, user.RoleName);
            return appUser.Id;
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            return await ToAppServiceUser(user);
        }

        public async Task<List<string>> GetUsernamesStartWith(string startWith)
        {
            return await _appIdentityDbContext.Users
                .Where(x => x.UserName.StartsWith(startWith))
                .Select(x => x.UserName)
                .ToListAsync();
        }

        private async Task<User> ToAppServiceUser(ApplicationUser user)
        {
            if (user == null)
            {
                return null;
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            // For now get allow the first role since one user has one role only
            string roleName = roles.First();
            return user.ToAppServiceUser(roleName);
        }

        public async Task<bool> IsHasUserAsync()
        {
            return await _appIdentityDbContext.Users.AnyAsync();
        }

        private string GetDefaultPassword()
        {
            return _appSettings.UserDefaultPassword;
        }
    }
}