using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
    }
}