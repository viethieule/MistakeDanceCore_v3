using API.Common;
using Application.Branches;
using Application.Common.Dropdowns;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BranchController : AuthenticatedController
{
    public BranchController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<IActionResult> DropdownOptions()
    {
        return Ok(await this.RunAsync<GetBranchOptionsService, DropdownOptionsRq, DropdownOptionsRs>(new DropdownOptionsRq()));
    }
}