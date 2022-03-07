using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Packages
{
    public class PackageDTC : DTCBase<Package, PackageDTO>
    {
        public PackageDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        public async Task UpdateRemainingSessionsByMemberIds(Dictionary<int, int> memberIdAndRemainingSessionDiffs)
        {
            List<int> memberIds = memberIdAndRemainingSessionDiffs.Keys.ToList();
            List<Package> packages = await _mistakeDanceDbContext.Packages.Where(x => memberIds.Contains(x.MemberId)).ToListAsync();
            foreach (Package package in packages)
            {
                package.RemainingSessions += memberIdAndRemainingSessionDiffs[package.MemberId];
            }

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        protected override void MapFromDTO(PackageDTO dto, Package efo)
        {
            throw new NotImplementedException();
        }

        protected override void MapToDTO(Package efo, PackageDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}