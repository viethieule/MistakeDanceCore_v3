using Application.Common;
using Application.Common.Interfaces;

namespace Application.Schedules
{
    public class CreateScheduleRq : BaseRequest
    {
    }

    public class CreateScheduleRs : BaseResponse
    {

    }

    public class CreateScheduleService : BaseService<CreateScheduleRq, CreateScheduleRs>
    {
        public CreateScheduleService(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        public override async Task<CreateScheduleRs> RunAsync(CreateScheduleRq rq)
        {
            return new CreateScheduleRs();
        }
    }
}