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
            efo.Id = dto.Id;
            efo.MemberId = dto.MemberId;
            efo.NumberOfSessions = dto.NumberOfSessions;
            efo.Price = dto.Price;
            efo.Months = dto.Months;
            efo.DefaultPackageId = dto.DefaultPackageId;
            efo.BranchRegisteredId = dto.BranchRegisteredId;
        }

        protected override void MapToDTO(Package efo, PackageDTO dto)
        {
            dto.Id = efo.Id;
            dto.MemberId = efo.MemberId;
            dto.NumberOfSessions = efo.NumberOfSessions;
            dto.Price = efo.Price;
            dto.Months = efo.Months;
            dto.DefaultPackageId = efo.DefaultPackageId;
            dto.BranchRegisteredId = efo.BranchRegisteredId;
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
            }

            await this.ValidateAndThrowAsync(dto);

            Package efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Packages.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
        }
    }
}