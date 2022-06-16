namespace Application.Memberships
{
    public class MembershipDTO
    {
        public int MemberId { get; set; }
        public int RemainingSessions { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsExpired
        {
            get
            {
                return RemainingSessions <= 0 || ExpiryDate < DateTime.Now.Date;
            }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}