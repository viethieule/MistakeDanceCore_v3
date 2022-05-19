using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace API.Common;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public abstract class AuthenticatedController : BaseApiController
{
    protected AuthenticatedController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}