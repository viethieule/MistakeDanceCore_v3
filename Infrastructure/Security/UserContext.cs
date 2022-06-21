using Application.Common.Interfaces;
using Application.Users;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            User = httpContextAccessor.HttpContext.Items[HttpContextCustomKey.User] as User;
        }

        public User User { get; }
    }
}