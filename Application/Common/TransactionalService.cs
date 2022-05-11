using Application.Common.Interfaces;

namespace Application.Common
{
    public abstract class TransactionalService<TRq, TRs> : BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        private readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        public TransactionalService(IMistakeDanceDbContext mistakeDanceDbContext)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;
        }

        protected sealed override async Task<TRs> DoRunAsync(TRq rq)
        {
            using (var transaction = await _mistakeDanceDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    TRs rs = await RunTransactionalAsync(rq);
                    await transaction.CommitAsync();
                    return rs;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        protected abstract Task<TRs> RunTransactionalAsync(TRq rq);
    }
}