using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Schedules
{
    public class ScheduleDTC : DTCBase<Schedule, ScheduleDTO>
    {
        public ScheduleDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        public async Task<ScheduleDTO> SingleWithIncludeByIdAsync(int id)
        {
            Schedule schedule = await _mistakeDanceDbContext.Schedules
                .Include(x => x.Branch)
                .Include(x => x.Class)
                .Include(x => x.Trainer)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (schedule == null)
            {
                throw new Exception("Schedule not exists");
            }

            return MapToDTO(schedule);
        }

        internal async Task<ScheduleDTO> SingleByIdAsync(int id)
        {
            Schedule schedule = await _mistakeDanceDbContext.Schedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (schedule == null)
            {
                throw new Exception("Schedule not exists");
            }

            return MapToDTO(schedule);
        }

        internal async Task CreateAsync(ScheduleDTO dto)
        {
            Schedule efo = MapFromDTO(dto);
            this.AuditOnCreate(efo);

            await _mistakeDanceDbContext.Schedules.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;

            MapToDTO(efo, dto);
        }

        internal async Task DeleteAsync(ScheduleDTO dto)
        {
            Schedule efo = MapFromDTO(dto);
            _mistakeDanceDbContext.Schedules.Attach(efo);
            _mistakeDanceDbContext.Entry(efo).State = EntityState.Deleted;

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        internal async Task UpdateAsync(ScheduleDTO dto)
        {
            ScheduleDTO currentDto = await SingleByIdAsync(dto.Id);
            if (dto.OpeningDate.Date != currentDto.OpeningDate.Date && currentDto.OpeningDate.Add(currentDto.StartTime) < DateTime.Now)
            {
                throw new Exception("Cannot update an already opened schedule");
            }

            Schedule efo = MapFromDTO(dto);
            
            _mistakeDanceDbContext.Schedules.Attach(efo);
            _mistakeDanceDbContext.Entry(efo).State = EntityState.Modified;
            this.AuditOnUpdate(efo);

            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;
        }

        protected override void MapFromDTO(ScheduleDTO dto, Schedule efo)
        {
            efo.Id = dto.Id;
            efo.Song = dto.Song;
            efo.OpeningDate = dto.OpeningDate;
            efo.StartTime = dto.StartTime;
            efo.DaysPerWeek = dto.DaysPerWeek;

            if (dto.BranchId.HasValue)
            {
                efo.BranchId = dto.BranchId.Value;
            }

            if (dto.ClassId.HasValue)
            {
                efo.ClassId = dto.ClassId.Value;
            }

            if (dto.TrainerId.HasValue)
            {
                efo.TrainerId = dto.TrainerId.Value;
            }
        }

        protected override void MapToDTO(Schedule efo, ScheduleDTO dto)
        {
            dto.Id = efo.Id;
            dto.Song = efo.Song;
            dto.OpeningDate = efo.OpeningDate;
            dto.StartTime = efo.StartTime;
            dto.DaysPerWeek = efo.DaysPerWeek;
            dto.TotalSessions = efo.TotalSessions;

            dto.BranchId = efo.BranchId;
            if (efo.Branch != null)
            {
                dto.BranchName = efo.Branch.Name;
            }

            dto.TrainerId = efo.TrainerId;
            if (efo.Trainer != null)
            {
                dto.TrainerName = efo.Trainer.Name;
            }

            dto.ClassId = efo.ClassId;
            if (efo.Class != null)
            {
                dto.ClassName = efo.Class.Name;
            }

            dto.CreatedDate = efo.CreatedDate;
            dto.CreatedBy = efo.CreatedBy;
            dto.UpdatedDate = efo.UpdatedDate;
            dto.UpdatedBy = efo.UpdatedBy;
        }
    }
}