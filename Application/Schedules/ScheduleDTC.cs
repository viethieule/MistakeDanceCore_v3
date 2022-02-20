using Application.Common;
using Application.Common.Interfaces;
using Application.Sessions;
using Domain;

namespace Application.Schedules
{
    public class ScheduleDTC : DTCBase<Schedule, ScheduleDTO>
    {
        private readonly SessionDTC _sessionDTC;

        public ScheduleDTC(IMistakeDanceDbContext mistakeDanceDbContext, SessionDTC sessionDTC) : base(mistakeDanceDbContext)
        {
            _sessionDTC = sessionDTC;
        }

        // public async Task<ScheduleFormDTO> CreateAsync(ScheduleFormDTO scheduleFormDTO)
        // {
        //     Schedule schedule = MapFromDTO(scheduleFormDTO.Schedule);

        //     if (schedule.TotalSessions.HasValue && schedule.DaysPerWeek.Count > 0)
        //     {
        //         schedule.Sessions = GenerateSessions(schedule);
        //     }
        //     else
        //     {
        //         schedule.Sessions = new List<Session>
        //         {
        //             new Session { Date = schedule.OpeningDate, Number = 1 }
        //         };
        //     }

        //     await _mistakeDanceDbContext.Schedules.AddAsync(schedule);
        //     await _mistakeDanceDbContext.SaveChangesAsync();

        //     scheduleFormDTO.Schedule.Id = schedule.Id;
        //     scheduleFormDTO.Schedule.BranchId = schedule.Branch.Id;
        //     scheduleFormDTO.Schedule.TrainerId = schedule.Trainer.Id;
        //     scheduleFormDTO.Schedule.ClassId = schedule.Class.Id;

        //     return scheduleFormDTO;
        // }

        public async Task<ScheduleFormDTO> CreateAsync(ScheduleFormDTO scheduleFormDTO)
        {
            ScheduleDTO dto = scheduleFormDTO.Schedule;
            if (!string.IsNullOrWhiteSpace(dto.BranchName))
            {
                BranchDTO branchDTO = _branchDTC.CreateAsync(new BranchDTO(dto.BranchName));
                dto.BranchId = branchDTO.Id;
            }

            if (!string.IsNullOrWhiteSpace(dto.ClassName))
            {
                ClassDTO classDTO = _classDTC.CreateAsync(new ClassDTO(dto.ClassName));
                dto.ClassId = classDTO.Id;
            }

            if (!string.IsNullOrWhiteSpace(dto.TrainerName))
            {
                TrainerDTO trainerDTO = _trainerDTC.CreateAsync(new dto.TrainerName)
                dto.TrainerId = trainerDTO.Id;
            }

            Schedule schedule = MapFromDTO(scheduleFormDTO.Schedule);
            dto.Id = schedule.Id;

            List<SessionDTO> sessionDTOs = _sessionDTC.CreateAsync(schedule);

            if (schedule.TotalSessions.HasValue && schedule.DaysPerWeek.Count > 0)
            {
                schedule.Sessions = GenerateSessions(schedule);
            }
            else
            {
                schedule.Sessions = new List<Session>
                {
                    new Session { Date = schedule.OpeningDate, Number = 1 }
                };
            }

            await _mistakeDanceDbContext.Schedules.AddAsync(schedule);
            await _mistakeDanceDbContext.SaveChangesAsync();

            scheduleFormDTO.Schedule.Id = schedule.Id;
            scheduleFormDTO.Schedule.BranchId = schedule.Branch.Id;
            scheduleFormDTO.Schedule.TrainerId = schedule.Trainer.Id;
            scheduleFormDTO.Schedule.ClassId = schedule.Class.Id;

            return scheduleFormDTO;
        }

        private List<Session> GenerateSessions(Schedule schedule)
        {
            List<Session> sessions = new List<Session>();

            if (!schedule.TotalSessions.HasValue || schedule.DaysPerWeek.Count == 0)
            {
                return sessions;
            }

            DateTime date = schedule.OpeningDate;
            int totalSessions = schedule.TotalSessions.Value;
            int[] recurDays = schedule.DaysPerWeek.Select(x => int.Parse(x.ToString())).ToArray();

            // To be validate when create / update schedule DTO
            int startIndex = Array.IndexOf(recurDays, (int)date.DayOfWeek);
            if (startIndex == -1)
            {
                return sessions;
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

            return sessions;
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
            else if (!string.IsNullOrWhiteSpace(dto.BranchName))
            {
                efo.Branch = new Branch { Name = dto.BranchName };
            }

            if (dto.ClassId.HasValue)
            {
                efo.ClassId = dto.ClassId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.ClassName))
            {
                efo.Class = new Class { Name = dto.ClassName };
            }

            if (dto.TrainerId.HasValue)
            {
                efo.TrainerId = dto.TrainerId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.TrainerName))
            {
                efo.Trainer = new Trainer { Name = dto.TrainerName };
            }
        }

        protected override void MapToDTO(Schedule efo, ScheduleDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}