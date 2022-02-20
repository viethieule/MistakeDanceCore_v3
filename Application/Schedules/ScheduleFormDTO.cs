using Application.Sessions;

namespace Application.Schedules
{
    public class ScheduleFormDTO
    {
        public ScheduleDTO Schedule { get; set; }
        public List<SessionDTO> Sessions { get; set; }
        public List<SessionDTO> SessionsCreatedInQueriedDateRange { get; set; }
    }
}