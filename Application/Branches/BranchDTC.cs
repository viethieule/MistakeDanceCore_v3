using Application.Common;
using Application.Common.Interfaces;
using Domain;

namespace Application.Branches
{
    public class BranchDTC : DTCBase<Branch, BranchDTO>
    {
        public BranchDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        public async Task CreateAsync(BranchDTO dto)
        {
            Branch efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Branches.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
        }

        protected override void MapFromDTO(BranchDTO dto, Branch efo)
        {
            efo.Id = dto.Id;
            efo.Name = dto.Name;
            efo.Abbreviation = dto.Abbreviation;
            efo.Address = dto.Address;
        }

        protected override void MapToDTO(Branch efo, BranchDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}