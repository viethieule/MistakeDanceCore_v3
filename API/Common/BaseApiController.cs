using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Common;

[Route("api/[controller]/[action]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    public BaseApiController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected async Task<TRs> RunAsync<TService, TRq, TRs>(TRq request)
        where TRq : BaseRequest
        where TRs : BaseResponse
        where TService : BaseService<TRq, TRs>
    {
        return await _serviceProvider.GetRequiredService<TService>().RunAsync(request);
    }
}
