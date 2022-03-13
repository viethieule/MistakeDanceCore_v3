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
            throw new NotImplementedException();
        }

        protected override void MapToDTO(Registration efo, RegistrationDTO dto)
        {
            throw new NotImplementedException();
        }

        internal async Task<List<RegistrationDTO>> ListShallowByScheduleIdAsync(int scheduleId)
        {
            List<Registration> registrations = await _mistakeDanceDbContext.Registrations
                .Where(x => x.Session.ScheduleId == scheduleId)
                .ToListAsync();

            return registrations.Select(MapToDTO).ToList();
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