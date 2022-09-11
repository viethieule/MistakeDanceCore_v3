using Application.Common;
using Application.Common.Dropdowns;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Classes
{
    public class ClassDTC : DTCBase<Class, ClassDTO>
    {
        public ClassDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        public async Task CreateAsync(ClassDTO dto)
        {
            Class efo = MapFromDTO(dto);
            this.AuditOnCreate(efo);
            await _mistakeDanceDbContext.Classes.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;

            dto.Id = efo.Id;
        }

        public async Task<List<DropdownOptionDTO>> GetDropdownOptions()
        {
            List<DropdownOptionDTO> options = await _mistakeDanceDbContext.Classes
                .Select(x => new DropdownOptionDTO(x.Id.ToString(), x.Name))
                .ToListAsync();

            return options;
        }

        protected override void MapFromDTO(ClassDTO dto, Class efo)
        {
            efo.Id = dto.Id;
            efo.Name = dto.Name;
        }

        protected override void MapToDTO(Class efo, ClassDTO dto)
        {
            dto.Id = efo.Id;
            dto.Name = efo.Name;
        }
    }
}