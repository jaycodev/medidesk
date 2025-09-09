namespace Shared.DTOs.Doctors.Responses
{
    public class DoctorResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public bool Status { get; set; }
    }
}
