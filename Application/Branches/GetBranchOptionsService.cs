using Application.Common;
using Application.Common.Dropdowns;
using Application.Common.Interfaces;

namespace Application.Branches;

public class GetBranchOptionsService : AuthenticatedService<DropdownOptionsRq, DropdownOptionsRs>
{
    private readonly BranchDTC _branchDTC;

    public GetBranchOptionsService(BranchDTC branchDTC, IUserContext userContext) : base(userContext)
    {
        _branchDTC = branchDTC;
    }

    protected override async Task<DropdownOptionsRs> DoRunAsync(DropdownOptionsRq rq)
    {
        DropdownOptionsRs rs = new()
        {
            Options = await _branchDTC.GetDropdownOptions()
        };

        return rs;
    }
}