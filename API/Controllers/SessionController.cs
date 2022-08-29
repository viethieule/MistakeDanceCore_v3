using API.Common;
using Application.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class SessionController : BaseApiController
{
    public SessionController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public async Task<IActionResult> GetByDateRange(ListSessionsRq rq)
    {
        return Ok(await this.RunAsync<ListSessionsService, ListSessionsRq, ListSessionsRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(DeleteSessionRq rq)
    {
        return Ok(await this.RunAsync<DeleteSessionService, DeleteSessionRq, DeleteSessionRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> Timetable(GetTimetableRq rq)
    {
        return Ok(await this.RunAsync<GetTimetableService, GetTimetableRq, GetTimetableRs>(rq));
    }
}
