using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Members
{
    public class MemberDTC : DTCBase<Member, MemberDTO>
    {
        public MemberDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        protected override void MapFromDTO(MemberDTO dto, Member efo)
        {
            efo.Id = dto.Id;
            efo.FullName = dto.FullName;
            efo.NormalizedFullName = dto.NormalizedFullName;
            efo.PhoneNumber = dto.PhoneNumber;
            efo.Birthdate = dto.Birthdate;
            efo.BranchId = dto.BranchId;
            efo.UserName = dto.UserName;
            efo.UserId = dto.UserId;
        }

        protected override void MapToDTO(Member efo, MemberDTO dto)
        {
            dto.Id = efo.Id;
            dto.FullName = efo.FullName;
            dto.NormalizedFullName = efo.NormalizedFullName;
            dto.PhoneNumber = efo.PhoneNumber;
            dto.Birthdate = efo.Birthdate;
            dto.BranchId = efo.BranchId;
            dto.UserName = efo.UserName;
            dto.UserId = efo.UserId;
            dto.CreatedDate = efo.CreatedDate;
            dto.CreatedBy = efo.CreatedBy;
            dto.UpdatedDate = efo.UpdatedDate;
            dto.UpdatedBy = efo.UpdatedBy;
        }

        internal async Task CreateAsync(MemberDTO dto)
        {
            Member efo = MapFromDTO(dto);

            this.AuditOnCreate(efo);

            await _mistakeDanceDbContext.Members.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;

            dto.Id = efo.Id;
        }

        internal async Task<List<MemberDTO>> ListAsync(GetMembersRq rq)
        {
            IQueryable<Member> query = _mistakeDanceDbContext.Members;

            if (!string.IsNullOrEmpty(rq.Name))
            {
                query = query.Where(u => u.FullName.Contains(rq.Name) || u.UserName.Contains(rq.Name));
            }

            if (!string.IsNullOrEmpty(rq.PhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber.Contains(rq.PhoneNumber));
            }
            
            if (rq.DefaultPackageId.HasValue)
            {
                bool isOtherPackage = rq.DefaultPackageId == -1;
                if (isOtherPackage)
                {
                    query = query.Where(u => u.Packages.Any(p => !p.DefaultPackageId.HasValue));
                }
                else
                {
                    query = query.Where(u => u.Packages.Any(p => p.DefaultPackageId.HasValue && p.DefaultPackageId == rq.DefaultPackageId));
                }
            }

            if (rq.CreatedDateFrom.HasValue)
            {
                query = query.Where(u => u.CreatedDate >= rq.CreatedDateFrom);
            }

            if (rq.CreatedDateTo.HasValue)
            {
                DateTime createdDateTo = rq.CreatedDateTo.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(u => u.CreatedDate <= createdDateTo);
            }

            if (rq.ExpiryDateFrom.HasValue)
            {
                query = query.Where(u => u.Membership.ExpiryDate >= rq.ExpiryDateFrom);
            }

            if (rq.ExpiryDateTo.HasValue)
            {
                DateTime expDateTo = rq.ExpiryDateTo.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(u => u.Membership.ExpiryDate <= expDateTo);
            }

            List<Member> members = await query.AsNoTracking().ToListAsync();

            return members.Select(MapToDTO).ToList();
        }

        internal async Task<MemberDTO> SingleByIdAsync(int id)
        {
            Member member = await _mistakeDanceDbContext.Members.AsNoTracking().SingleAsync(x => x.Id == id);
            return MapToDTO(member);
        }
    }
}