using API.Common;
using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : AuthenticatedController
    {
        public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Current()
        {
            GetCurrentUserRq rq = new();
            return Ok(await this.RunAsync<GetCurrentUserService, GetCurrentUserRq, GetCurrentUserRs>(rq));
        }
    }
}