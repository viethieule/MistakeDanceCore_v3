using Application.Common.Interfaces;
using Domain;

namespace Application.Common
{
    public abstract class DTCBase<TENT, TDTO>
        where TENT : BaseEntity, new()
        where TDTO : class, new()
    {
        private readonly IMistakeDanceDbContext _mistakeDanceDbContext;
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

        private TENT MapFromDTO(TDTO dto)
        {
            TENT efo = new TENT();
            MapFromDTO(dto, efo);
            return efo;
        }

        private TDTO MapToDTO(TENT efo)
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