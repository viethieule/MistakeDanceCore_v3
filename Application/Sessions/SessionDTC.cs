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
        public SessionDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        internal async Task<List<SessionDTO>> ListAsync(DateTime start, DateTime end)
        {
            List<Session> sessions = await _mistakeDanceDbContext.Sessions
                .Where(x => x.Date <= end && x.Date >= start)
                .Include(x => x.Schedule).ThenInclude(x => x.Branch)
                .Include(x => x.Schedule).ThenInclude(x => x.Trainer)
                .Include(x => x.Schedule).ThenInclude(x => x.Class)
                .Include(x => x.Registrations)
                .AsNoTracking()
                .ToListAsync();

            return sessions.Select(MapToDTO).ToList();
        }

        internal async Task<List<SessionDTO>> ListByScheduleIdAsync(int scheduleId)
        {
            List<Session> sessions = await _mistakeDanceDbContext.Sessions
                .Where(x => x.ScheduleId == scheduleId)
                .Include(x => x.Schedule).ThenInclude(x => x.Branch)
                .Include(x => x.Schedule).ThenInclude(x => x.Trainer)
                .Include(x => x.Schedule).ThenInclude(x => x.Class)
                .AsNoTracking()
                .ToListAsync();
            return sessions.Select(MapToDTO).ToList();
        }

        internal async Task<List<SessionDTO>> ListShallowByScheduleIdAsync(int scheduleId)
        {
            List<Session> sessions = await _mistakeDanceDbContext.Sessions.AsNoTracking().Where(x => x.ScheduleId == scheduleId).ToListAsync();
            return sessions.Select(MapToDTO).ToList();
        }

        internal async Task CreateAsync(SessionDTO dto)
        {
            Session efo = MapFromDTO(dto);
            this.AuditOnCreate(efo);
            await _mistakeDanceDbContext.Sessions.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;

            dto.Id = efo.Id;
        }

        internal async Task CreateRangeAsync(List<SessionDTO> dtos)
        {
            List<Session> efos = dtos.Select(MapFromDTO).ToList();

            await _mistakeDanceDbContext.Sessions.AddRangeAsync(efos);
            await _mistakeDanceDbContext.SaveChangesAsync();

            efos.ForEach(efo => _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached);

            dtos.ForEach(dto => dto.Id = efos.First(efo => efo.Number == dto.Number).Id);
        }

        internal async Task<SessionDTO> SingleWithScheduleByIdAsync(int id)
        {
            Session session = await _mistakeDanceDbContext.Sessions.Include(x => x.Schedule).AsNoTracking().SingleAsync(x => x.Id == id);
            return MapToDTO(session);
        }

        internal async Task<SessionDTO> SingleByIdAsync(int id)
        {
            Session session = await _mistakeDanceDbContext.Sessions.AsNoTracking().SingleAsync(x => x.Id == id);
            return MapToDTO(session);
        }

        // TODO: Refactor so that less loops?
        internal async Task UpdateRangeAsync(List<SessionDTO> dtos)
        {
            List<Session> efos = dtos.Select(MapFromDTO).ToList();

            foreach (Session efo in efos)
            {
                _mistakeDanceDbContext.Sessions.Attach(efo);
                _mistakeDanceDbContext.Entry(efo).State = EntityState.Modified;
                this.AuditOnUpdate(efo);
            }

            await _mistakeDanceDbContext.SaveChangesAsync();

            efos.ForEach(efo => _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached);

            dtos.ForEach(dto => dto.Id = efos.First(efo => efo.Number == dto.Number).Id);
        }

        internal async Task<List<SessionDTO>> ListFollowingSessions(SessionDTO sessionDTO)
        {
            List<Session> sessions = await _mistakeDanceDbContext.Sessions
                .Where(x => x.ScheduleId == sessionDTO.ScheduleId && x.Id > sessionDTO.Id)
                .AsNoTracking()
                .ToListAsync();

            return sessions.Select(MapToDTO).ToList();
        }

        internal async Task DeleteRangeAsync(List<SessionDTO> dtos)
        {
            foreach (SessionDTO dto in dtos)
            {
                Session efo = MapFromDTO(dto);
                _mistakeDanceDbContext.Sessions.Attach(efo);
                _mistakeDanceDbContext.Entry(efo).State = EntityState.Deleted;
            }

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        internal async Task RebuildScheduleSessionsNumberAsync(List<SessionDTO> sessions)
        {
            sessions = sessions.OrderBy(x => x.Date).ToList();
            for (int i = 0; i < sessions.Count; i++)
            {
                sessions[i].Number = i + 1;
            }

            await this.UpdateRangeAsync(sessions);
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