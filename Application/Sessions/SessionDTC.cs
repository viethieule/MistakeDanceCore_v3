using Application.Common;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Schedules;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Sessions
{
    public class SessionDTC : DTCBase<Session, SessionDTO>
    {
        public SessionDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        public async Task<List<SessionDTO>> ListAsync(DateTime start, DateTime end)
        {
            List<Session> sessions = await _mistakeDanceDbContext.Sessions
                .Where(x => x.Date <= end && x.Date >= start)
                .Include(x => x.Schedule).ThenInclude(x => x.Branch)
                .Include(x => x.Schedule).ThenInclude(x => x.Trainer)
                .Include(x => x.Schedule).ThenInclude(x => x.Class)
                .Include(x => x.Registrations)
                .ToListAsync();

            return sessions.Select(MapToDTO).ToList();
        }

        public async Task CreateAsync(SessionDTO dto)
        {
            Session efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Sessions.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
        }

        public async Task CreateRangeAsync(List<SessionDTO> dtos)
        {
            List<Session> efos = dtos.Select(MapFromDTO).ToList();

            await _mistakeDanceDbContext.Sessions.AddRangeAsync(efos);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dtos.Zip(efos, (dto, efo) =>
            {
                dto.Id = efo.Id;
                return dto;
            });
        }

        public async Task DeleteRangeAsync(List<SessionDTO> dtos)
        {
            foreach (SessionDTO dto in dtos)
            {
                Session efo = MapFromDTO(dto);
                _mistakeDanceDbContext.Sessions.Attach(efo);
                _mistakeDanceDbContext.Entry(efo).State = EntityState.Deleted;
            }

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        public async Task<List<SessionDTO>> GetByScheduleIdAsync(int scheduleId)
        {
            List<Session> sessions = await _mistakeDanceDbContext.Sessions.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            return sessions.Select(MapToDTO).ToList();
        }

        protected override void MapFromDTO(SessionDTO dto, Session efo)
        {
            efo.Id = dto.Id;
            efo.Date = dto.Date;
            efo.Number = dto.Number;
            efo.ScheduleId = dto.ScheduleId;
        }

        protected override void MapToDTO(Session efo, SessionDTO dto)
        {
            dto.Id = efo.Id;
            dto.Date = efo.Date;
            dto.Number = efo.Number;
            dto.ScheduleId = efo.ScheduleId;
            dto.TotalRegistered = efo.Registrations.Count;

            if (efo.Schedule != null)
            {
                Schedule schedule = efo.Schedule;
                dto.Song = schedule.Song;
                dto.OpeningDate = schedule.OpeningDate;
                dto.DaysPerWeek = schedule.DaysPerWeek;
                dto.TotalSessions = schedule.TotalSessions;
                dto.StartTime = schedule.StartTime;
                dto.BranchId = schedule.BranchId;
                dto.BranchName = schedule.Branch != null ? schedule.Branch.Name : string.Empty;
                dto.ClassId = schedule.ClassId;
                dto.ClassName = schedule.Class != null ? schedule.Class.Name : string.Empty;
                dto.TrainerId = schedule.TrainerId;
                dto.TrainerName = schedule.Trainer != null ? schedule.Trainer.Name : string.Empty;
            }
        }
    }
}