using Application.Branches;
using Application.Classes;
using Application.Common;
using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Sessions;
using Application.Trainers;
using AutoMapper;

namespace Application.Schedules
{
    public class CreateScheduleRq : BaseRequest
    {
        public ScheduleFormDTO ScheduleFormDTO { get; set; }
    }

    public class CreateScheduleRs : BaseResponse
    {
        public List<SessionDTO> SessionsCreated { get; set; }
    }

    public class CreateScheduleService : TransactionalService<CreateScheduleRq, CreateScheduleRs>
    {
        private readonly BranchDTC _branchDTC;
        private readonly TrainerDTC _trainerDTC;
        private readonly ClassDTC _classDTC;
        private readonly ScheduleDTC _scheduleDTC;
        private readonly SessionDTC _sessionDTC;

        public CreateScheduleService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            BranchDTC branchDTC,
            TrainerDTC trainerDTC,
            ClassDTC classDTC,
            ScheduleDTC scheduleDTC,
            SessionDTC sessionDTC) : base(mistakeDanceDbContext)
        {
            _branchDTC = branchDTC;
            _trainerDTC = trainerDTC;
            _classDTC = classDTC;
            _scheduleDTC = scheduleDTC;
            _sessionDTC = sessionDTC;
        }

        protected override async Task<CreateScheduleRs> RunTransactionalAsync(CreateScheduleRq rq)
        {
            ScheduleFormDTO scheduleFormDTO = rq.ScheduleFormDTO;
            ScheduleDTO scheduleDto = scheduleFormDTO.Schedule;

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

            await _scheduleDTC.CreateAsync(scheduleDto);

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

            return new CreateScheduleRs
            {
                SessionsCreated = scheduleFormDTO.Sessions
            };
        }
    }
}