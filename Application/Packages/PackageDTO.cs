namespace Application.Packages
{
    public class PackageDTO
    {
        public int Id { get; set; }
        public int MemberId { get; set; }

        public int NumberOfSessions { get; set; }
        public double Price { get; set; }
        public int Months { get; set; }

        public int RemainingSessions { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int? DefaultPackageId { get; set; }

        public int BranchRegisteredId { get; set; }
    }
}