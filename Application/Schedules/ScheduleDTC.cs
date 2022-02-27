using Application.Branches;
using Application.Classes;
using Application.Common;
using Application.Common.Interfaces;
using Application.Sessions;
using Application.Trainers;
using FluentValidation;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Schedules
{
    public class ScheduleDTC : DTCBase<Schedule, ScheduleDTO>
    {
        private readonly SessionDTC _sessionDTC;
        private readonly BranchDTC _branchDTC;
        private readonly ClassDTC _classDTC;
        private readonly TrainerDTC _trainerDTC;

        public ScheduleDTC(
            IMistakeDanceDbContext mistakeDanceDbContext,
            SessionDTC sessionDTC,
            BranchDTC branchDTC,
            ClassDTC classDTC,
            TrainerDTC trainerDTC) : base(mistakeDanceDbContext)
        {
            _sessionDTC = sessionDTC;
            _branchDTC = branchDTC;
            _classDTC = classDTC;
            _trainerDTC = trainerDTC;
        }

        public async Task<ScheduleFormDTO> CreateAsync(ScheduleFormDTO scheduleFormDTO)
        {
            ScheduleDTO scheduleDto = scheduleFormDTO.Schedule;
            
            await RunTransactionalAsync(async () =>
            {
                if (!string.IsNullOrWhiteSpace(scheduleDto.BranchName))
                {
                    BranchDTO branchDTO = new() { Name = scheduleDto.BranchName };
                    await _branchDTC.CreateAsync(branchDTO);
                    scheduleDto.BranchId = branchDTO.Id;
                }

                if (!string.IsNullOrWhiteSpace(scheduleDto.ClassName))
                {
                    ClassDTO classDTO = new() { Name = scheduleDto.ClassName };
                    await _classDTC.CreateAsync(classDTO);
                    scheduleDto.ClassId = classDTO.Id;
                }

                if (!string.IsNullOrWhiteSpace(scheduleDto.TrainerName))
                {
                    TrainerDTO trainerDTO = new() { Name = scheduleDto.TrainerName };
                    await _trainerDTC.CreateAsync(trainerDTO);
                    scheduleDto.TrainerId = trainerDTO.Id;
                }

                await CreateAsync(scheduleDto);

                if (scheduleDto.TotalSessions.HasValue && scheduleDto.DaysPerWeek.Count > 0)
                {
                    scheduleFormDTO.Sessions.AddRange(await _sessionDTC.CreateRangeAsync(scheduleDto));
                }
                else
                {
                    SessionDTO sessionDto = new SessionDTO
                    {
                        Date = scheduleDto.OpeningDate,
                        Number = 1,
                        ScheduleId = scheduleDto.Id
                    };
                    await _sessionDTC.CreateAsync(sessionDto);

                    scheduleFormDTO.Sessions.Add(sessionDto);
                }
            });

            return scheduleFormDTO;
        }

        private async Task CreateAsync(ScheduleDTO dto)
        {
            Schedule efo = MapFromDTO(dto);
            await _mistakeDanceDbContext.Schedules.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            dto.Id = efo.Id;
        }

        public async Task UpdateAsync(ScheduleDTO dto)
        {
            await this.ValidateAndThrowAsync(dto);
            
            Schedule efo = MapFromDTO(dto);
            _mistakeDanceDbContext.Schedules.Attach(efo);
            _mistakeDanceDbContext.Entry(efo).State = EntityState.Modified;
            await _mistakeDanceDbContext.SaveChangesAsync();
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
            throw new NotImplementedException();
        }
    }
}