using Application.Common.Interfaces;
using Domain;

namespace Application.Common
{
    public abstract class DTCBase<TENT, TDTO>
        where TENT : BaseEntity, new()
        where TDTO : class, new()
    {
        protected readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        public DTCBase(IMistakeDanceDbContext mistakeDanceDbContext)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;

            SetValidationRules();
        }

        protected virtual void ValidateAndThrow(TDTO dto)
        {
        }

        protected virtual void SetValidationRules()
        {
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