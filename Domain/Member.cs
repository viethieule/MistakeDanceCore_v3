namespace Domain
{
    public class Member : BaseEntity
    {
        public string FullName { get; set; }
        public string NormalizedFullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public List<Registration> Registrations { get; set; }
        public List<Package> Packages { get; set; }
        public Branch Branch { get; set; }
        public int BranchId { get; set; }
        public Membership Membership { get; set; }
    }
}