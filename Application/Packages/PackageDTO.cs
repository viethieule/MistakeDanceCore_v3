namespace Application.Packages
{
    public class PackageDTO
    {
        public int Id { get; set; }
        public int MemberId { get; set; }

        public int NumberOfSessions { get; set; }
        public double Price { get; set; }
        public int Months { get; set; }

        public int? DefaultPackageId { get; set; }

        public int BranchRegisteredId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}