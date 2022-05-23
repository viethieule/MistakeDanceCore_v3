using Application.Common;
using Application.Common.Interfaces;
using Domain;

namespace Application.Branches
{
    public class BranchDTC : DTCBase<Branch, BranchDTO>
    {
        public BranchDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        public async Task CreateAsync(BranchDTO dto)
        {
            Branch efo = MapFromDTO(dto);
            this.AuditOnCreate(efo);
            await _mistakeDanceDbContext.Branches.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            MapToDTO(efo, dto);
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
            dto.Id = efo.Id;
            dto.Name = efo.Name;
            dto.Abbreviation = efo.Abbreviation;
            dto.Address = efo.Address;
        }
    }
}