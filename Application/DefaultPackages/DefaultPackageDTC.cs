using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.DefaultPackages
{
    public class DefaultPackageDTC : DTCBase<DefaultPackage, DefaultPackageDTO>
    {
        public DefaultPackageDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        protected override void MapFromDTO(DefaultPackageDTO dto, DefaultPackage efo)
        {
            throw new NotImplementedException();
        }

        protected override void MapToDTO(DefaultPackage efo, DefaultPackageDTO dto)
        {
            throw new NotImplementedException();
        }

        internal async Task<DefaultPackageDTO> SingleByIdAsync(int id)
        {
            DefaultPackage defaultPackage = await _mistakeDanceDbContext.DefaultPackages.SingleAsync(x => x.Id == id);
            return MapToDTO(defaultPackage);
        }
    }
}