namespace MistakeDance.Domain
{
    public class Schedule : BaseEntity
    {
        public string Song { get; set; }
        public DateTime OpeningDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<DayOfWeek> DaysPerWeek { get; set; } = new List<DayOfWeek>();
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public List<Session> Sessions { get; set; }
    }
}