namespace Domain
{
    public class Membership : BaseEntity
    {
        public int MemberId { get; set; }

        public int RemainingSessions { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}