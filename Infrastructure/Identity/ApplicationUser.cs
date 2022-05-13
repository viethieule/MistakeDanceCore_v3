using Application.Users;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(string username)
        {
            this.UserName = username;
        }

        public User ToAppServiceUser(string roleName)
        {
            return new User
            {
                Id = this.Id,
                UserName = this.UserName,
                RoleName = roleName
            };
        }
    }
}