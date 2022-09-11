using Application.Common;
using Application.Common.Dropdowns;
using Application.Common.Interfaces;

namespace Application.Classes;

public class GetClassOptionsService : AuthenticatedService<DropdownOptionsRq, DropdownOptionsRs>
{
    private readonly ClassDTC _classDTC;

    public GetClassOptionsService(ClassDTC classDTC, IUserContext userContext) : base(userContext)
    {
        _classDTC = classDTC;
    }

    protected override async Task<DropdownOptionsRs> DoRunAsync(DropdownOptionsRq rq)
    {
        DropdownOptionsRs rs = new()
        {
            Options = await _classDTC.GetDropdownOptions()
        };

        return rs;
    }
}