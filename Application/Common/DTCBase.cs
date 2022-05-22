using Application.Common.Interfaces;
using Application.Users;
using Domain;

namespace Application.Common
{
    public abstract class DTCBase<TENT, TDTO>
        where TENT : class, new()
        where TDTO : class, new()
    {
        private readonly IUserContext _userContext;
        protected readonly IMistakeDanceDbContext _mistakeDanceDbContext;
        private IMistakeDanceDbContext mistakeDanceDbContext;

        protected User User => _userContext.User;

        public DTCBase(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext)
        {
            _mistakeDanceDbContext = mistakeDanceDbContext;
            _userContext = userContext;
        }

        protected void AuditOnCreate(TENT ent)
        {
            if (ent is IAuditable)
            {
                IAuditable auditableEnt = (IAuditable)ent;
                auditableEnt.CreatedBy = this.User.UserName;
                auditableEnt.CreatedDate = DateTime.Now;
                auditableEnt.UpdatedBy = this.User.UserName;
                auditableEnt.UpdatedDate = DateTime.Now;
            }
        }

        protected void AuditOnUpdate(TENT ent)
        {
            if (ent is IAuditable auditableEnt)
            {
                auditableEnt.UpdatedBy = this.User.UserName;
                auditableEnt.UpdatedDate = DateTime.Now;
                _mistakeDanceDbContext.Entry(ent).Property(nameof(auditableEnt.CreatedBy)).IsModified = false;
                _mistakeDanceDbContext.Entry(ent).Property(nameof(auditableEnt.CreatedDate)).IsModified = false;
            }
        }

        protected TENT MapFromDTO(TDTO dto)
        {
            TENT efo = new TENT();
            MapFromDTO(dto, efo);
            return efo;
        }

        protected TDTO MapToDTO(TENT efo)
        {
            if (efo == null)
            {
                return null;
            }

            TDTO dto = new TDTO();
            MapToDTO(efo, dto);
            return dto;
        }

        protected abstract void MapToDTO(TENT efo, TDTO dto);
        protected abstract void MapFromDTO(TDTO dto, TENT efo);
    }
}