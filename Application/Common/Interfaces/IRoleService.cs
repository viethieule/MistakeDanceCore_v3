namespace Application.Common.Interfaces
{
    public interface IRoleService
    {
        Task CreateRolesAsync(params string[] list);
        Task<bool> IsHasRoleAsync();
    }
}