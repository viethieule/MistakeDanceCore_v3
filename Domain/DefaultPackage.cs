namespace Domain
{
    public class DefaultPackage : AuditableBaseEntity
    {
        public int NumberOfSessions { get; set; }
        public double Price { get; set; }
        public int Months { get; set; }
    }
}