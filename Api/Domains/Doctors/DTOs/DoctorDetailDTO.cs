namespace Api.Domains.Doctors.DTOs
{
    public class DoctorDetailDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; }
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
        public string ProfilePicture { get; set; }
        public bool Status { get; set; }
    }
}
