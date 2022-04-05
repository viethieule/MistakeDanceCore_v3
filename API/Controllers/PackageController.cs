using API.Common;
using Application.Packages;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class PackageController : BaseApiController
{
    public PackageController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddPackageRq rq)
    {
        return Ok(await this.RunAsync<AddPackageService, AddPackageRq, AddPackageRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> ListByMember(ListPackageByMemberRq rq)
    {
        return Ok(await this.RunAsync<ListPackageByMemberService, ListPackageByMemberRq, ListPackageByMemberRs>(rq));
    }
}
