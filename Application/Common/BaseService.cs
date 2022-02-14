using Application.Common.Interfaces;

namespace Application.Common
{
    public abstract class BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        private readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        public BaseService(IMistakeDanceDbContext mistakeDanceDbContext)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;
        }

        public abstract Task<TRs> RunAsync(TRq rq);
    }
}