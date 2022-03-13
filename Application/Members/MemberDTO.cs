namespace Application.Members
{
    public class MemberDTO
    {
        public string FullName { get; set; }
        public string NormalizedFullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Birthdate { get; set; }
        public int BranchId { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}