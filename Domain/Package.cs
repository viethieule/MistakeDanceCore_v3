namespace Domain
{
    public class Package : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int NumberOfSessions { get; set; }
        public double Price { get; set; }
        public int Months { get; set; }

        public int RemainingSessions { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int? DefaultPackageId { get; set; }
        public DefaultPackage DefaultPackage { get; set; }
    }
}