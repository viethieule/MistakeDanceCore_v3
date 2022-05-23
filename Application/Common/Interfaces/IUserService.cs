using Application.Users;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<string> CreateWithRoleAsync(User user);
        Task<List<string>> GetUsernamesStartWith(string startWith);
        Task CreateManyWithRoleAsync(List<User> users);
        Task<bool> IsHasUserAsync();
        Task<User> FindByUsernameAsync(string username);
        Task<bool> CheckPasswordSigninAsync(string username, string password);
    }
}