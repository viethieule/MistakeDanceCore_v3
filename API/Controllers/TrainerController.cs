using API.Common;
using Application.Common.Dropdowns;
using Application.Trainers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TrainerController : AuthenticatedController
{
    public TrainerController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    public async Task<IActionResult> DropdownOptions()
    {
        return Ok(await this.RunAsync<GetTrainerOptionsService, DropdownOptionsRq, DropdownOptionsRs>(new DropdownOptionsRq()));
    }
}