namespace Domain
{
    public class Membership
    {
        public int MemberId { get; set; }

        public int RemainingSessions { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}