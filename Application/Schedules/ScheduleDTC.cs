using Application.Branches;
using Application.Classes;
using Application.Common;
using Application.Common.Interfaces;
using Application.Sessions;
using Application.Trainers;
using FluentValidation;
using Domain;
using Microsoft.EntityFrameworkCore;
using Application.Common.Helpers;
using Application.Registrations;
using Application.Packages;
using Application.Memberships;

namespace Application.Schedules
{
    public class ScheduleDTC : DTCBase<Schedule, ScheduleDTO>
    {
        private readonly SessionDTC _sessionDTC;
        private readonly BranchDTC _branchDTC;
        private readonly ClassDTC _classDTC;
        private readonly TrainerDTC _trainerDTC;
        private readonly RegistrationDTC _registrationDTC;
        private readonly PackageDTC _packageDTC;
        private readonly MembershipDTC _membershipDTC;

        public ScheduleDTC(
            IMistakeDanceDbContext mistakeDanceDbContext,
            SessionDTC sessionDTC,
            BranchDTC branchDTC,
            ClassDTC classDTC,
            TrainerDTC trainerDTC,
            RegistrationDTC registrationDTC,
            PackageDTC packageDTC,
            MembershipDTC membershipDTC) : base(mistakeDanceDbContext)
        {
            _sessionDTC = sessionDTC;
            _branchDTC = branchDTC;
            _classDTC = classDTC;
            _trainerDTC = trainerDTC;
            _registrationDTC = registrationDTC;
            _packageDTC = packageDTC;
            _membershipDTC = membershipDTC;
        }

        public async Task<ScheduleDTO> GetByIdAsync(int id, bool throwIfEmpty = false)
        {
            Schedule schedule = await _mistakeDanceDbContext.Schedules.FirstOrDefaultAsync(x => x.Id == id);
            if (schedule == null)
            {
                return !throwIfEmpty ? null : throw new Exception("Schedule not exists");
            }

            return MapToDTO(schedule);
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
                    List<SessionDTO> sessionDTOs = SessionsGenerator.Generate(scheduleDto);
                    scheduleFormDTO.Sessions.AddRange(sessionDTOs);
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
            ScheduleDTO currentDto = await GetByIdAsync(dto.Id, true);
            if (dto.OpeningDate.Date != currentDto.OpeningDate.Date && currentDto.OpeningDate.Add(currentDto.StartTime) < DateTime.Now)
            {
                throw new Exception("Cannot update an already opened schedule");
            }

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
            dto.Id = efo.Id;
            dto.Song = efo.Song;
            dto.OpeningDate = efo.OpeningDate;
            dto.StartTime = efo.StartTime;
            dto.DaysPerWeek = efo.DaysPerWeek;

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
        }
    }
}