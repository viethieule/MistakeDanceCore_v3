using Application.Common;
using Application.Common.Interfaces;
using Application.Sessions;
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

    public class CreateScheduleService : BaseService<CreateScheduleRq, CreateScheduleRs>
    {
        private readonly ScheduleDTC _scheduleDTC;
        public CreateScheduleService(ScheduleDTC scheduleDTC)
        {
            _scheduleDTC = scheduleDTC;
        }
        public override async Task<CreateScheduleRs> RunAsync(CreateScheduleRq rq)
        {
            ScheduleFormDTO dto = await _scheduleDTC.CreateAsync(rq.ScheduleFormDTO);
            
            return new CreateScheduleRs
            {
                SessionsCreated = dto.SessionsCreatedInQueriedDateRange
            };
        }
    }
}