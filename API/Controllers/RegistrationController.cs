using API.Common;
using Application.Registrations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class RegistrationController : BaseApiController
{
    public RegistrationController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRegistrationRq rq)
    {
        return Ok(await this.RunAsync<CreateRegistrationService, CreateRegistrationRq, CreateRegistrationRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(CancelRegistrationRq rq)
    {
        return Ok(await this.RunAsync<CancelRegistrationService, CancelRegistrationRq, CancelRegistrationRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> ListBySessionId(ListRegistrationsBySessionIdRq rq)
    {
        return Ok(await this.RunAsync<ListRegistrationsBySessionIdService, ListRegistrationsBySessionIdRq, ListRegistrationsBySessionIdRs>(rq));
    }
}
