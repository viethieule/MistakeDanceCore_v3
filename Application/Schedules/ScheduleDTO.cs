namespace Application.Schedules
{
    public class ScheduleDTO
    {
        public int Id { get; set; }
        public string Song { get; set; }
        public DateTime OpeningDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<DayOfWeek> DaysPerWeek { get; set; } = new List<DayOfWeek>();
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public int? TrainerId { get; set; }
        public string TrainerName { get; set; }
        public int? TotalSessions { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}