using Application.Users;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<string> CreateWithRoleAsync(User user);
        Task<List<string>> GetUsernamesStartWith(string startWith);
        Task<User> GetCurrentUser();
    }
}