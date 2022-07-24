using API.Common;
using Application.Schedules;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ScheduleController : AuthenticatedController
{
    public ScheduleController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateScheduleRq rq)
    {
        return Ok(await this.RunAsync<CreateScheduleService, CreateScheduleRq, CreateScheduleRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateScheduleRq rq)
    {
        return Ok(await this.RunAsync<UpdateScheduleService, UpdateScheduleRq, UpdateScheduleRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(DeleteScheduleRq rq)
    {
        return Ok(await this.RunAsync<DeleteScheduleService, DeleteScheduleRq, DeleteScheduleRs>(rq));
    }
}
