using Application.Common.Interfaces;
using AutoMapper;

namespace Application.Common
{
    public abstract class BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        public BaseService()
        {
        }

        protected abstract Task<TRs> RunAsync(TRq rq);
    }
}