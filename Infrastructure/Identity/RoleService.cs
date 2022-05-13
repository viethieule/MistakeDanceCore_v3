using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationIdentityDbContext _appIdentityDbContext;
        public RoleService(RoleManager<IdentityRole> roleManager, ApplicationIdentityDbContext appIdentityDbContext)
        {
            _appIdentityDbContext = appIdentityDbContext;
            _roleManager = roleManager;

        }

        public async Task CreateRolesAsync(params string[] roleNames)
        {
            foreach (string name in roleNames)
            {
                IdentityRole role = new IdentityRole(name);
                await _roleManager.CreateAsync(role);
            }
        }

        public async Task<bool> IsHasRoleAsync()
        {
            return await _appIdentityDbContext.Roles.AnyAsync();
        }
    }
}