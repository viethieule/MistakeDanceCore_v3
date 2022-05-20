using Application.Schedules;

namespace Application.Sessions
{
    public class SessionDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public int ScheduleId { get; set; }
        public string Song { get; set; }
        public DateTime OpeningDate { get; set; }
        public List<DayOfWeek> DaysPerWeek { get; set; }
        public int? TotalSessions { get; set; }
        public TimeSpan StartTime { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public int TotalRegistered { get; set; }
    }
}