using Application.Users;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public User ToAppServiceUser(string roleName)
        {
            return new User
            {
                Id = this.Id,
                UserName = this.UserName
            };
        }
    }
}