using Application.Users;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<string> CreateWithRoleAsync(User user);
        Task<List<string>> GetUsernamesStartWith(string startWith);
        Task<User> GetCurrentUser();
        Task CreateManyWithRoleAsync(List<User> users);
        Task<bool> IsHasUserAsync();
        Task CreateRolesAsync(params string[] list);
        Task<bool> IsHasRoleAsync();
    }
}