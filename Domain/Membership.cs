namespace Domain
{
    public class Membership : IAuditable
    {
        public int MemberId { get; set; }

        public int RemainingSessions { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}