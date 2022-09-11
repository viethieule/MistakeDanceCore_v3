using API.Common;
using Application.Common.Dropdowns;
using Application.Classes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ClassController : AuthenticatedController
{
    public ClassController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<IActionResult> DropdownOptions()
    {
        return Ok(await this.RunAsync<GetClassOptionsService, DropdownOptionsRq, DropdownOptionsRs>(new DropdownOptionsRq()));
    }
}