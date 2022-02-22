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

        public async Task CreateAsync(SessionDTO dto)
        {
            Session efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Sessions.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
        }

        public async Task<List<SessionDTO>> CreateRangeAsync(ScheduleDTO schedule)
        {
            List<Session> sessions = new List<Session>();
            if (!schedule.TotalSessions.HasValue || schedule.DaysPerWeek.Count == 0)
            {
                return new List<SessionDTO>();
            }

            DateTime date = schedule.OpeningDate;
            int totalSessions = schedule.TotalSessions.Value;
            int[] recurDays = schedule.DaysPerWeek.Select(x => int.Parse(x.ToString())).ToArray();

            // To be validate when create / update schedule DTO
            int startIndex = Array.IndexOf(recurDays, (int)date.DayOfWeek);
            if (startIndex == -1)
            {
                return new List<SessionDTO>();
            }

            for (int i = startIndex, j = 1; i >= -1 && j > 0; i++, j++)
            {
                sessions.Add(new Session
                {
                    ScheduleId = schedule.Id,
                    Date = date,
                    Number = j
                });

                if (sessions.Count == totalSessions)
                {
                    break;
                }

                if (i == recurDays.Length - 1)
                {
                    date = date.AddDays(7 - (recurDays[i] - recurDays[0]));
                    i = -1;
                }
                else
                {
                    date = date.AddDays(recurDays[i + 1] - recurDays[i]);
                }
            }

            await _mistakeDanceDbContext.Sessions.AddRangeAsync(sessions);
            await _mistakeDanceDbContext.SaveChangesAsync();

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