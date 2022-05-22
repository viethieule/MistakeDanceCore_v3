using Application.Common;
using Application.Common.Interfaces;
using Domain;

namespace Application.Trainers
{
    public class TrainerDTC : DTCBase<Trainer, TrainerDTO>
    {
        public TrainerDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        internal async Task CreateAsync(TrainerDTO dto)
        {
            Trainer efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Trainers.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

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