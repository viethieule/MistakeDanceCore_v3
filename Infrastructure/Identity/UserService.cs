using Application.Common.Interfaces;
using Application.Users;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public Task<int> CreateWithRoleAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetUsernamesStartWith(string startWith)
        {
            throw new NotImplementedException();
        }
    }
}