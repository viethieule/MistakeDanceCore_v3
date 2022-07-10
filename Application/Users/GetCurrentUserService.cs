using Application.Common;
using Application.Common.Interfaces;

namespace Application.Users
{
    public class GetCurrentUserRq : BaseRequest
    {
    }

    public class GetCurrentUserRs : BaseResponse
    {
        public User User { get; set; }
    }

    public class GetCurrentUserService : AuthenticatedService<GetCurrentUserRq, GetCurrentUserRs>
    {
        public GetCurrentUserService(IUserContext userContext) : base(userContext)
        {
        }

        protected override async Task<GetCurrentUserRs> DoRunAsync(GetCurrentUserRq rq)
        {
            return await Task.FromResult(new GetCurrentUserRs
            {
                User = User
            });
        }
    }
}