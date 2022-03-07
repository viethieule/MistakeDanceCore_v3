using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Memberships
{
    public class MembershipDTC : DTCBase<Membership, MembershipDTO>
    {
        public MembershipDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
        {
        }

        public async Task UpdateRemainingSessionsByMemberIds(Dictionary<int, int> memberIdAndRemainingSessionDiffs)
        {
            List<int> memberIds = memberIdAndRemainingSessionDiffs.Keys.ToList();
            List<Membership> memberships = await _mistakeDanceDbContext.Memberships.Where(x => memberIds.Contains(x.MemberId)).ToListAsync();

            foreach (Membership membership in memberships)
            {
                membership.RemainingSessions += memberIdAndRemainingSessionDiffs[membership.MemberId];
            }

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        protected override void MapFromDTO(MembershipDTO dto, Membership efo)
        {
            throw new NotImplementedException();
        }

        protected override void MapToDTO(Membership efo, MembershipDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}