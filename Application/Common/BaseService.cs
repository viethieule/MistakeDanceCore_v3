namespace Application.Common
{
    public abstract class BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        public BaseService()
        {
        }

        public abstract Task<TRs> RunAsync(TRq rq);
    }
}