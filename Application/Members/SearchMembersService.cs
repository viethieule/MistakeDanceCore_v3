using Application.Common;
using Application.Common.Interfaces;

namespace Application.Members;

public class SearchMembersRq : BaseRequest
{
    public string Keyword { get; set; }
}

public class SearchMembersRs : BaseResponse
{
    public List<MemberDTO> Members { get; set; }
}

public class SearchMembersService : AuthenticatedService<SearchMembersRq, SearchMembersRs>
{
    private readonly MemberDTC _memberDTC;

    public SearchMembersService(MemberDTC memberDTC, IUserContext userContext) : base(userContext)
    {
        _memberDTC = memberDTC;
    }

    protected override async Task<SearchMembersRs> DoRunAsync(SearchMembersRq rq)
    {
        List<MemberDTO> members = await _memberDTC.Search(rq.Keyword);
        return new SearchMembersRs
        {
            Members = members
        };
    }
}
