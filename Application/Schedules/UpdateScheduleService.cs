using Application.Branches;
using Application.Classes;
using Application.Common;
using Application.Common.Interfaces;
using Application.Trainers;

namespace Application.Schedules
{
    public class UpdateScheduleRq : BaseRequest
    {
        public ScheduleFormDTO ScheduleFormDTO { get; set; }
    }

    public class UpdateScheduleRs : BaseResponse
    {
    }

    public class UpdateScheduleService : BaseService<UpdateScheduleRq, UpdateScheduleRs>
    {
        private readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        private readonly BranchDTC _branchDTC;
        private readonly TrainerDTC _trainerDTC;
        private readonly ClassDTC _classDTC;

        public UpdateScheduleService(
            IMistakeDanceDbContext mistakeDanceDbContext,
            BranchDTC branchDTC,
            TrainerDTC trainerDTC,
            ClassDTC classDTC)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;
            _branchDTC = branchDTC;
            _trainerDTC = trainerDTC;
            _classDTC = classDTC;
        }

        public override async Task<UpdateScheduleRs> RunAsync(UpdateScheduleRq rq)
        {
            using (var transaction = await _mistakeDanceDbContext.Database.BeginTransactionAsync())
            {
                ScheduleDTO scheduleDto = rq.ScheduleFormDTO.Schedule;

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
            }
        }
    }
}