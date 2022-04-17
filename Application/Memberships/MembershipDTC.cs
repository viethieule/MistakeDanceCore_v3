using Application.Common;
using Application.Common.Interfaces;
using Domain;
using FluentValidation;
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
            throw new NotImplementedException();
        }

        protected override void MapToDTO(Membership efo, MembershipDTO dto)
        {
            throw new NotImplementedException();
        }

        internal async Task CreateAsync(MembershipDTO dto)
        {
            await this.ValidateAndThrowAsync(dto);
            Membership efo = MapFromDTO(dto);

            await _mistakeDanceDbContext.Memberships.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        internal async Task UpdateAsync(MembershipDTO dto)
        {
            await this.ValidateAndThrowAsync(dto);
            Membership efo = MapFromDTO(dto);
            
            _mistakeDanceDbContext.Memberships.Attach(efo);
            _mistakeDanceDbContext.Entry(efo).State = EntityState.Modified;

            await _mistakeDanceDbContext.SaveChangesAsync();
        }
    }
}