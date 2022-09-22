using Application.Common;

namespace Application.Registrations;

public class ListRegistrationsBySessionIdRq : BaseRequest
{
    public int SessionId { get; set; }
}

public class ListRegistrationsBySessionIdRs : BaseResponse
{
    public List<RegistrationDTO> Registrations { get; set; }
}

public class ListRegistrationsBySessionIdService : BaseService<ListRegistrationsBySessionIdRq, ListRegistrationsBySessionIdRs>
{
    private readonly RegistrationDTC _registrationDTC;
    public ListRegistrationsBySessionIdService(RegistrationDTC registrationDTC)
    {
        _registrationDTC = registrationDTC;
    }

    protected override async Task<ListRegistrationsBySessionIdRs> DoRunAsync(ListRegistrationsBySessionIdRq rq)
    {
        List<RegistrationDTO> registrationDTOs = await _registrationDTC.ListBySessionIdAsync(rq.SessionId);
        return new ListRegistrationsBySessionIdRs
        {
            Registrations = registrationDTOs
        };
    }
}



