using Application.Common.Interfaces;
using Application.Users;

namespace Application.Common
{
    public abstract class AuthenticatedService<TRq, TRs> : BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        private readonly IUserContext _userContext;
        protected User User => _userContext.User;
        public AuthenticatedService(IUserContext userContext)
        {
            _userContext = userContext;
        }
    }
}