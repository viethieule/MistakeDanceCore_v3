using Application.Common.Interfaces;
using AutoMapper;

namespace Application.Common
{
    public abstract class BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        protected readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        protected readonly IMapper _mapper;
        public BaseService(IMistakeDanceDbContext mistakeDanceDbContext, IMapper mapper)
        {
            _mapper = mapper;
            _mistakeDanceDbContext = mistakeDanceDbContext;
        }

        public abstract Task<TRs> RunAsync(TRq rq);
    }
}