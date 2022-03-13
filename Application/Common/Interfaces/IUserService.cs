using Application.Users;

namespace Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateWithRoleAsync(UserDTO user);
        Task<List<string>> GetUsernamesStartWith(string startWith);
        Task<UserDTO> GetCurrentUser();
    }
}