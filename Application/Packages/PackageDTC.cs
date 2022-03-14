using Application.Common;
using Application.Common.Interfaces;
using Application.DefaultPackages;
using Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Packages
{
    public class PackageDTC : DTCBase<Package, PackageDTO>
    {
        private readonly DefaultPackageDTC _defaultPackageDTC;

        public PackageDTC(IMistakeDanceDbContext mistakeDanceDbContext,
            DefaultPackageDTC defaultPackageDTC) : base(mistakeDanceDbContext)
        {
            _defaultPackageDTC = defaultPackageDTC;
        }

        protected override void MapFromDTO(PackageDTO dto, Package efo)
        {
            throw new NotImplementedException();
        }

        protected override void MapToDTO(Package efo, PackageDTO dto)
        {
            throw new NotImplementedException();
        }

        internal async Task<List<PackageDTO>> ListByMemberIdAsync(int memberId)
        {
            List<Package> packages = await _mistakeDanceDbContext.Packages.Where(x => x.MemberId == memberId).ToListAsync();
            return packages.Select(MapToDTO).ToList();
        }

        internal async Task CreateAsync(PackageDTO dto)
        {
            if (dto.DefaultPackageId.HasValue)
            {
                DefaultPackageDTO defaultPackageDTO = await _defaultPackageDTC.SingleByIdAsync(dto.DefaultPackageId.Value);
                dto.NumberOfSessions = defaultPackageDTO.NumberOfSessions;
                dto.Months = defaultPackageDTO.Months;
                dto.Price = defaultPackageDTO.Price;
                dto.RemainingSessions = defaultPackageDTO.NumberOfSessions;
            }
            else
            {
                dto.RemainingSessions = dto.NumberOfSessions;
            }

            dto.ExpiryDate = DateTime.Now.AddMonths(dto.Months);

            await this.ValidateAndThrowAsync(dto);

            Package efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Packages.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
        }
    }
}