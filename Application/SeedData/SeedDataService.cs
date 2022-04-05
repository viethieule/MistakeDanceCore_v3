using Application.Common;

namespace Application.SeedData
{
    public class SeedDataRq : BaseRequest
    {
    }

    public class SeedDataRs : BaseResponse
    {
    }

    public class SeedDataService : BaseService<SeedDataRq, SeedDataRs>
    {
        public override Task<SeedDataRs> RunAsync(SeedDataRq rq)
        {
            throw new NotImplementedException();
        }
    }
}