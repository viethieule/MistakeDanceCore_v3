using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Registrations
{
    public class RegistrationDTC : DTCBase<Registration, RegistrationDTO>
    {
        public RegistrationDTC(IMistakeDanceDbContext mistakeDanceDbContext, IUserContext userContext) : base(mistakeDanceDbContext, userContext)
        {
        }

        internal async Task<List<RegistrationDTO>> GetBySessionIdsAsync(List<int> sessionIds)
        {
            List<Registration> registrations = await _mistakeDanceDbContext.Registrations
                .Where(x => sessionIds.Contains(x.SessionId))
                .AsNoTracking()
                .ToListAsync();

            return registrations.Select(MapToDTO).ToList();
        }

        internal async Task DeleteRangeAsync(List<RegistrationDTO> dtos)
        {
            foreach (RegistrationDTO dto in dtos)
            {
                Registration efo = MapFromDTO(dto);
                _mistakeDanceDbContext.Registrations.Attach(efo);
                _mistakeDanceDbContext.Entry(efo).State = EntityState.Deleted;
            }

            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        protected override void MapFromDTO(RegistrationDTO dto, Registration efo)
        {
            efo.Id = dto.Id;
            efo.SessionId = dto.SessionId;
            efo.MemberId = dto.MemberId;
            efo.Status = dto.Status;
        }

        protected override void MapToDTO(Registration efo, RegistrationDTO dto)
        {
            dto.Id = efo.Id;
            dto.SessionId = efo.SessionId;
            dto.MemberId = efo.MemberId;
            dto.Status = efo.Status;
            dto.CreatedDate = efo.CreatedDate;
            dto.CreatedBy = efo.CreatedBy;
            dto.UpdatedDate = efo.UpdatedDate;
            dto.UpdatedBy = efo.UpdatedBy;
        }

        internal async Task<RegistrationDTO> SingleByIdAsync(int id)
        {
            Registration efo = await _mistakeDanceDbContext.Registrations.AsNoTracking().SingleAsync(x => x.Id == id);
            return MapToDTO(efo);
        }

        internal async Task<List<RegistrationDTO>> ListShallowByScheduleIdAsync(int scheduleId)
        {
            List<Registration> efos = await _mistakeDanceDbContext.Registrations
                .Where(x => x.Session.ScheduleId == scheduleId)
                .AsNoTracking()
                .ToListAsync();

            return efos.Select(MapToDTO).ToList();
        }

        internal async Task CreateAsync(RegistrationDTO registrationDTO)
        {
            Registration efo = MapFromDTO(registrationDTO);
            this.AuditOnCreate(efo);
            await _mistakeDanceDbContext.Registrations.AddAsync(efo);
            await _mistakeDanceDbContext.SaveChangesAsync();

            _mistakeDanceDbContext.Entry(efo).State = EntityState.Detached;

            registrationDTO.Id = efo.Id;
        }

        internal async Task DeleteAsync(RegistrationDTO dto)
        {
            Registration efo = MapFromDTO(dto);
            _mistakeDanceDbContext.Registrations.Attach(efo);
            _mistakeDanceDbContext.Entry(efo).State = EntityState.Deleted;
            await _mistakeDanceDbContext.SaveChangesAsync();
        }

        internal async Task<List<RegistrationDTO>> ListShallowBySessionIdsAsync(IEnumerable<int> sessionIds)
        {
            List<Registration> registrations = await _mistakeDanceDbContext.Registrations
                .Where(x => sessionIds.Contains(x.Id))
                .AsNoTracking()
                .ToListAsync();

            return registrations.Select(MapToDTO).ToList();
        }
    }
}