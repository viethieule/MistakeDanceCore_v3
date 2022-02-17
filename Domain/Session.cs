namespace Domain
{
    public class Session : BaseEntity
    {
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public List<Registration> Registrations { get; set; } = new();
    }
}