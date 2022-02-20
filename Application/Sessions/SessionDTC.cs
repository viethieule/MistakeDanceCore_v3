using Application.Common;
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

        protected override void MapFromDTO(SessionDTO dto, Session efo)
        {
            throw new NotImplementedException();
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