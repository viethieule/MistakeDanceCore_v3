using Application.Common;
using Application.Common.Interfaces;
using Domain;

namespace Application.Classes
{
    public class ClassDTC : DTCBase<Class, ClassDTO>
    {
        public ClassDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        public async Task CreateAsync(ClassDTO dto)
        {
            Class efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Classes.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
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