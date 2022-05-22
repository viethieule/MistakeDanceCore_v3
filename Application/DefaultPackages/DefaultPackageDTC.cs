using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.DefaultPackages
{
    public class DefaultPackageDTC : DTCBase<DefaultPackage, DefaultPackageDTO>
    {
        public DefaultPackageDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        protected override void MapFromDTO(DefaultPackageDTO dto, DefaultPackage efo)
        {
            efo.Id = dto.Id;
            efo.NumberOfSessions = dto.NumberOfSessions;
            efo.Months = dto.Months;
            efo.Price = dto.Price;
        }

        protected override void MapToDTO(DefaultPackage efo, DefaultPackageDTO dto)
        {
            dto.Id = efo.Id;
            dto.NumberOfSessions = efo.NumberOfSessions;
            dto.Months = efo.Months;
            dto.Price = efo.Price;
        }

        internal async Task<DefaultPackageDTO> SingleByIdAsync(int id)
        {
            DefaultPackage defaultPackage = await _mistakeDanceDbContext.DefaultPackages.SingleAsync(x => x.Id == id);
            return MapToDTO(defaultPackage);
        }
    }
}