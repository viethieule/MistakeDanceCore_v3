namespace Domain
{
    public class Branch : AuditableBaseEntity
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
    }
}