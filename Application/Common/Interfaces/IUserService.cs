using Application.Users;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateWithRoleAsync(User user);
        Task<List<string>> GetUsernamesStartWith(string startWith);
        Task<User> GetCurrentUser();
    }
}