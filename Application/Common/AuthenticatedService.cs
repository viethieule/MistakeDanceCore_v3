using Application.Common.Interfaces;

namespace Application.Common
{
    public abstract class AuthenticatedService<TRq, TRs> : BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        private readonly IUserContext _userContext;
        public AuthenticatedService(IUserContext userContext)
        {
            _userContext = userContext;
        }
    }
}