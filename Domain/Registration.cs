namespace Domain
{
    public class Registration : BaseEntity
    {
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public RegistrationStatus Status { get; set; }
    }

    public enum RegistrationStatus
    {
        Registered,
        Attended
    }
}