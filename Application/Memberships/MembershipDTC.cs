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

        internal async Task UpdateRemainingSessionsByMemberIds(Dictionary<int, int> memberIdAndRemainingSessionDiffs)
        {
            List<int> memberIds = memberIdAndRemainingSessionDiffs.Keys.ToList();
            List<Membership> memberships = await _mistakeDanceDbContext.Memberships.Where(x => memberIds.Contains(x.MemberId)).ToListAsync();

            foreach (Membership membership in memberships)
            {
                membership.RemainingSessions += memberIdAndRemainingSessionDiffs[membership.MemberId];
            }

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        internal async Task<MembershipDTO> SingleByMemberIdAsync(int memberId)
        {
            Membership membership = await _mistakeDanceDbContext.Memberships.SingleAsync(x => x.MemberId == memberId);
            return MapToDTO(membership);
        }

        protected override void MapFromDTO(MembershipDTO dto, Membership efo)
        {
            efo.MemberId = dto.MemberId;
            efo.ExpiryDate = dto.ExpiryDate;
            efo.RemainingSessions = dto.RemainingSessions;
        }

        protected override void MapToDTO(Membership efo, MembershipDTO dto)
        {
            dto.MemberId = efo.MemberId;
            dto.ExpiryDate = efo.ExpiryDate;
            dto.RemainingSessions = efo.RemainingSessions;
        }

        internal async Task CreateAsync(MembershipDTO dto)
        {
            Membership efo = MapFromDTO(dto);

            await _mistakeDanceDbContext.Memberships.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        internal async Task UpdateAsync(MembershipDTO dto)
        {
            Membership efo = MapFromDTO(dto);
            
            _mistakeDanceDbContext.Memberships.Attach(efo);
            _mistakeDanceDbContext.Entry(efo).State = EntityState.Modified;

            await _mistakeDanceDbContext.SaveChangesAsync();
        }
    }
}