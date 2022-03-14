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
    }
}