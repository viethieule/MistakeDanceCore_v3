namespace Domain
{
    public class Member : BaseEntity, IAuditable
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
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}