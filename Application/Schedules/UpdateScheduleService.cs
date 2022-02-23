using Application.Common;

namespace Application.Schedules
{
    public class UpdateScheduleRq : BaseRequest
    {
    }

    public class UpdateScheduleRs : BaseResponse
    {
    }

    public class UpdateScheduleService : BaseService<UpdateScheduleRq, UpdateScheduleRs>
    {
        public override Task<UpdateScheduleRs> RunAsync(UpdateScheduleRq rq)
        {
            throw new NotImplementedException();
        }
    }
}