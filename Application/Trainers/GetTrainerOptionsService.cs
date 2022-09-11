using Application.Common;
using Application.Common.Dropdowns;
using Application.Common.Interfaces;

namespace Application.Trainers;

public class GetTrainerOptionsService : AuthenticatedService<DropdownOptionsRq, DropdownOptionsRs>
{
    private readonly TrainerDTC _trainerDTC;

    public GetTrainerOptionsService(TrainerDTC trainerDTC, IUserContext userContext) : base(userContext)
    {
        _trainerDTC = trainerDTC;
    }

    protected override async Task<DropdownOptionsRs> DoRunAsync(DropdownOptionsRq rq)
    {
        DropdownOptionsRs rs = new()
        {
            Options = await _trainerDTC.GetDropdownOptions()  
        };

        return rs;
    }
}