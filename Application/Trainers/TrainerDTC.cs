using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Trainers
{
    public class TrainerDTC : DTCBase<Trainer, TrainerDTO>
    {
        public TrainerDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        public async Task CreateAsync(TrainerDTO dto)
        {
            Trainer efo = MapFromDTO(dto);
            this.AuditOnCreate(efo);
            await _mistakeDanceDbContext.Trainers.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;

            dto.Id = efo.Id;
        }

        protected override void MapFromDTO(TrainerDTO dto, Trainer efo)
        {
            efo.Id = dto.Id;
            efo.Name = dto.Name;
        }

        protected override void MapToDTO(Trainer efo, TrainerDTO dto)
        {
            dto.Id = efo.Id;
            dto.Name = efo.Name;
        }
    }
}