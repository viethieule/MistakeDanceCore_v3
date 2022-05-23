using Domain;

namespace Application.Registrations
{
    public class RegistrationDTO
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int MemberId { get; set; }
        public RegistrationStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}