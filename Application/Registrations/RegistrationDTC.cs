using Application.Common;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Registrations
{
    public class RegistrationDTC : DTCBase<Registration, RegistrationDTO>
    {
        public RegistrationDTC(IMistakeDanceDbContext mistakeDanceDbContext) : base(mistakeDanceDbContext)
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
        }

        internal async Task<RegistrationDTO> SingleByIdAsync(int id)
        {
            Registration efo = await _mistakeDanceDbContext.Registrations.SingleAsync(x => x.Id == id);
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
            Registration registration = MapFromDTO(registrationDTO);

            await _mistakeDanceDbContext.Registrations.AddAsync(registration);
            await _mistakeDanceDbContext.SaveChangesAsync();

            registrationDTO.Id = registration.Id;
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
                .ToListAsync();

            return registrations.Select(MapToDTO).ToList();
        }
    }
}