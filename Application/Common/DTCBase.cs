using Application.Common.Interfaces;
using Domain;
using FluentValidation;

namespace Application.Common
{
    public abstract class DTCBase<TENT, TDTO> : AbstractValidator<TDTO>
        where TENT : BaseEntity, new()
        where TDTO : class, new()
    {
        protected readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        public DTCBase(IMistakeDanceDbContext mistakeDanceDbContext)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;

            SetValidationRules();
        }

        protected virtual void SetValidationRules()
        {
        }

        protected async Task RunTransactionalAsync(Func<Task> action)
        {
            using (var transaction = await _mistakeDanceDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await action.Invoke();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        protected TENT MapFromDTO(TDTO dto)
        {
            TENT efo = new TENT();
            MapFromDTO(dto, efo);
            return efo;
        }

        protected TDTO MapToDTO(TENT efo)
        {
            if (efo == null)
            {
                return null;
            }

            TDTO dto = new TDTO();
            MapToDTO(efo, dto);
            return dto;
        }

        protected abstract void MapToDTO(TENT efo, TDTO dto);
        protected abstract void MapFromDTO(TDTO dto, TENT efo);
    }
}