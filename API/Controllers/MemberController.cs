using API.Common;
using Application.Members;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class MemberController : AuthenticatedController
{
    public MemberController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberRq rq)
    {
        return Ok(await this.RunAsync<CreateMemberService, CreateMemberRq, CreateMemberRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> GetAll(GetMembersRq rq)
    {
        return Ok(await this.RunAsync<GetMembersService, GetMembersRq, GetMembersRs>(rq));
    }

    [HttpPost]
    public async Task<IActionResult> Search(SearchMembersRq rq)
    {
        return Ok(await this.RunAsync<SearchMembersService, SearchMembersRq, SearchMembersRs>(rq));
    }
}