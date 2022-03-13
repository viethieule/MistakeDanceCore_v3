using Application.Common;

namespace Application.Members
{
    public class GetMembersRq : BaseRequest
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int? DefaultPackageId { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public DateTime? ExpiryDateFrom { get; set; }
        public DateTime? ExpiryDateTo { get; set; }
    }

    public class GetMembersRs : BaseResponse
    {
        public List<MemberDTO> Members { get; set; }
    }

    public class GetMembersService : BaseService<GetMembersRq, GetMembersRs>
    {
        private readonly MemberDTC _memberDTC;
        public GetMembersService(MemberDTC memberDTC)
        {
            _memberDTC = memberDTC;

        }
        public override async Task<GetMembersRs> RunAsync(GetMembersRq rq)
        {
            GetMembersRs rs = new GetMembersRs();
            rs.Members = await _memberDTC.ListAsync(rq);
            return rs;
        }
    }
}